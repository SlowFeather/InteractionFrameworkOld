using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace InteractionFramework.Editor
{
    public static class AddDebugHelp 
    {
        [MenuItem("Interaction Framework/Debug Help/Add Debug Window",false, 30)]
        //将Debug物体添加到场景
        public static void AddDebugFunction()
        {
            string path = "Assets/Plugins/IngameDebugConsole/IngameDebugConsole.prefab";
            GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go!=null)
            {
                GameObject gameObject= GameObject.Instantiate(go);
                gameObject.name = "IngameDebugConsole";
                gameObject.transform.SetSiblingIndex(999);
            }
            else
            {
                Debug.LogError("Load IngameDebugConsole.prefab Error!");
            }
            
        }
    }
}