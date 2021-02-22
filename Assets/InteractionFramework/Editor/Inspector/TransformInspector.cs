using UnityEngine;
using UnityEditor;
using System.Text;
namespace InteractionFramework.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Transform), true)]
	public class TransformInspector : UnityEditor.Editor
	{
		static public TransformInspector instance;

		//当前的本地坐标
		SerializedProperty mPos;
		//当前的本地旋转
		SerializedProperty mRot;
		//当前的本地缩放
		SerializedProperty mScale;

		void OnEnable()
		{
			instance = this;

			if (this)
			{
				try
				{
					var so = serializedObject;
					mPos = so.FindProperty("m_LocalPosition");
					mRot = so.FindProperty("m_LocalRotation");
					mScale = so.FindProperty("m_LocalScale");
				}
				catch { }
			}
		}

		void OnDestroy() { instance = null; }

		/// <summary>
		/// Draw the inspector widget.绘制inspector小部件。
		/// </summary>
		public override void OnInspectorGUI()
		{
			//设置label的宽度
			EditorGUIUtility.labelWidth = 15f;

			serializedObject.Update();

			DrawPosition();
			DrawRotation();
			DrawScale();
			DrawCopyAndPaste();

			serializedObject.ApplyModifiedProperties();
		}


		void DrawCopyAndPaste()
		{
			GUILayout.BeginHorizontal();
			bool reset = GUILayout.Button("Copy");
			bool reset2 = GUILayout.Button("Paste");
			bool reset3 = GUILayout.Button("ReSet");
			GUILayout.EndHorizontal();

			if (reset)
			{
				//把数值打印出来
				var select = Selection.activeGameObject;
				if (select == null)
					return;
				//Debug.Log(select.name+"("+ mPos.vector3Value.x.ToString()+ ","+ mPos.vector3Value.y.ToString() + ","+ mPos.vector3Value.z.ToString() + ")");
				//Debug.Log(select.name + mRot.quaternionValue);
				//Debug.Log(select.name + "(" + mScale.vector3Value.x.ToString() + "," + mScale.vector3Value.y.ToString() + "," + mScale.vector3Value.z.ToString() + ")");

				StringBuilder s = new StringBuilder();
				s.Append("TransformInspector_" + "(" + mPos.vector3Value.x.ToString() + "," + mPos.vector3Value.y.ToString() + "," + mPos.vector3Value.z.ToString() + ")" + "_");
				s.Append(mRot.quaternionValue + "_");
				s.Append("(" + mScale.vector3Value.x.ToString() + "," + mScale.vector3Value.y.ToString() + "," + mScale.vector3Value.z.ToString() + ")");
				//添加到剪贴板
				UnityEngine.GUIUtility.systemCopyBuffer = s.ToString();
			}
			if (reset2)
			{
				//把数值打印出来
				//Debug.Log(UnityEngine.GUIUtility.systemCopyBuffer);
				string s = UnityEngine.GUIUtility.systemCopyBuffer;
				string[] sArr = s.Split('_');
				if (sArr[0] != "TransformInspector")
				{
					Debug.LogError("未复制Transform组件内容！Transform component content not copied!");
					return;
				}
				//Debug.Log("Pos:" + sArr[1]);
				//Debug.Log("Rot:" + sArr[2]);
				//Debug.Log("Scale:" + sArr[3]);
				try
				{
					mPos.vector3Value = ParseV3(sArr[1]);
					mRot.quaternionValue = new Quaternion() { x = ParseV4(sArr[2]).x, y = ParseV4(sArr[2]).y, z = ParseV4(sArr[2]).z, w = ParseV4(sArr[2]).w };
					mScale.vector3Value = ParseV3(sArr[3]);
				}
				catch (System.Exception ex)
				{
					Debug.LogError(ex);
					return;
				}

			}
            if (reset3)
            {
				mPos.vector3Value = Vector3.zero;
				mRot.quaternionValue = new Quaternion() { eulerAngles= Vector3.zero } ;
				mScale.vector3Value = Vector3.one;

			}
		}
		/// <summary>
		/// String To Vector3
		/// </summary>
		/// <param name="strVector3"></param>
		/// <returns></returns>
		Vector3 ParseV3(string strVector3)
		{
			strVector3 = strVector3.Replace("(", "").Replace(")", "");
			string[] s = strVector3.Split(',');
			return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
		}
		/// <summary>
		/// String To Vector4
		/// </summary>
		/// <param name="strVector4"></param>
		/// <returns></returns>
		Vector4 ParseV4(string strVector4)
		{
			strVector4 = strVector4.Replace("(", "").Replace(")", "");
			string[] s = strVector4.Split(',');
			return new Vector4(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));
		}
		#region Position 位置
		void DrawPosition()
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(mPos.FindPropertyRelative("x"));
			EditorGUILayout.PropertyField(mPos.FindPropertyRelative("y"));
			EditorGUILayout.PropertyField(mPos.FindPropertyRelative("z"));
			bool reset = GUILayout.Button("P", GUILayout.Width(20f));

			GUILayout.EndHorizontal();

			if (reset) mPos.vector3Value = Vector3.zero;
		}
		#endregion
		#region Scale 缩放
		void DrawScale()
		{
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(mScale.FindPropertyRelative("x"));
				EditorGUILayout.PropertyField(mScale.FindPropertyRelative("y"));
				EditorGUILayout.PropertyField(mScale.FindPropertyRelative("z"));
				bool reset = GUILayout.Button("S", GUILayout.Width(20f));
				if (reset) mScale.vector3Value = Vector3.one;
			}
			GUILayout.EndHorizontal();
		}
		#endregion
		#region Rotation is ugly as hell... since there is no native support for quaternion property drawing 旋转是丑陋的地狱。。。因为四元数属性绘图没有本地支持
		enum Axes : int
		{
			None = 0,
			X = 1,
			Y = 2,
			Z = 4,
			All = 7,
		}

		Axes CheckDifference(Transform t, Vector3 original)
		{
			Vector3 next = t.localEulerAngles;

			Axes axes = Axes.None;

			if (Differs(next.x, original.x)) axes |= Axes.X;
			if (Differs(next.y, original.y)) axes |= Axes.Y;
			if (Differs(next.z, original.z)) axes |= Axes.Z;

			return axes;
		}

		Axes CheckDifference(SerializedProperty property)
		{
			Axes axes = Axes.None;

			if (property.hasMultipleDifferentValues)
			{
				Vector3 original = property.quaternionValue.eulerAngles;

				foreach (Object obj in serializedObject.targetObjects)
				{
					axes |= CheckDifference(obj as Transform, original);
					if (axes == Axes.All) break;
				}
			}
			return axes;
		}

		/// <summary>
		/// Draw an editable float field. 绘制可编辑的浮动字段。
		/// </summary>
		/// <param name="hidden">Whether to replace the value with a dash 是否将值替换为破折号</param>
		/// <param name="greyedOut">Whether the value should be greyed out or not 值是否应灰显</param>
		static bool FloatField(string name, ref float value, bool hidden, GUILayoutOption opt)
		{
			float newValue = value;
			GUI.changed = false;

			if (!hidden)
			{
				newValue = EditorGUILayout.FloatField(name, newValue, opt);
			}
			else
			{
				float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
			}

			if (GUI.changed && Differs(newValue, value))
			{
				value = newValue;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Because Mathf.Approximately is too sensitive.因为数学近似值太敏感了。
		/// </summary>
		static bool Differs(float a, float b) { return Mathf.Abs(a - b) > 0.0001f; }

		/// <summary>
		/// 注册Undo
		/// </summary>
		/// <param name="name"></param>
		/// <param name="objects"></param>
		static public void RegisterUndo(string name, params Object[] objects)
		{
			if (objects != null && objects.Length > 0)
			{
				UnityEditor.Undo.RecordObjects(objects, name);

				foreach (Object obj in objects)
				{
					if (obj == null) continue;
					EditorUtility.SetDirty(obj);
				}
			}
		}

		/// <summary>
		/// 角度处理
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		static public float WrapAngle(float angle)
		{
			while (angle > 180f) angle -= 360f;
			while (angle < -180f) angle += 360f;
			return angle;
		}

		void DrawRotation()
		{
			GUILayout.BeginHorizontal();
			{
				Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;

				visible.x = WrapAngle(visible.x);
				visible.y = WrapAngle(visible.y);
				visible.z = WrapAngle(visible.z);

				Axes changed = CheckDifference(mRot);
				Axes altered = Axes.None;

				GUILayoutOption opt = GUILayout.MinWidth(30f);

				if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, opt)) altered |= Axes.X;
				if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, opt)) altered |= Axes.Y;
				if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, opt)) altered |= Axes.Z;
				bool reset = GUILayout.Button("R", GUILayout.Width(20f));

				if (reset)
				{
					mRot.quaternionValue = Quaternion.identity;
				}
				else if (altered != Axes.None)
				{
					RegisterUndo("Change Rotation", serializedObject.targetObjects);

					foreach (Object obj in serializedObject.targetObjects)
					{
						Transform t = obj as Transform;
						Vector3 v = t.localEulerAngles;

						if ((altered & Axes.X) != 0) v.x = visible.x;
						if ((altered & Axes.Y) != 0) v.y = visible.y;
						if ((altered & Axes.Z) != 0) v.z = visible.z;

						t.localEulerAngles = v;
					}
				}
			}
			GUILayout.EndHorizontal();
		}
		#endregion
	}

}
