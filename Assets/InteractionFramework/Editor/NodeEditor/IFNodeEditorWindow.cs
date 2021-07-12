using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IFNodeEditorWindow : EditorWindow
{


    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    Vector2 mousePosition=Vector2.zero;

    private GUIStyle nodeStyle;


    [MenuItem("Interaction Framework/IFNodeEditor", false, 1)]
    static void ShowIFEditor()
    {
        IFNodeEditorWindow window = (IFNodeEditorWindow)EditorWindow.GetWindow(typeof(IFNodeEditorWindow));
        window.Show();
    }
    

    private void OnEnable()
    {
        titleContent.text = "IFNodeEditor Window";

        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

    }
    private void OnGUI()
    {

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();


        

        ShowNotification(new GUIContent("Hello","tip"), 2);

        RemoveNotification();
    }
    void Update()
    {
        if (Event.current==null)
        {
            return;
        }
        if (Event.current.type == EventType.MouseMove)
        {
            mousePosition = Event.current.mousePosition;
            EditorGUILayout.LabelField("Mouse Position: ", mousePosition.ToString());
            Repaint();
        }

            
    }
    /// <summary>
    /// 每秒10帧的速度刷新
    /// </summary>
    void OnInspectorUpdate()
    {

        //EditorGUILayout.LabelField("Mouse Position: ", mousePosition.ToString());

        //Focus() 获取焦点
        //Repaint() 重新绘制
    }





}
