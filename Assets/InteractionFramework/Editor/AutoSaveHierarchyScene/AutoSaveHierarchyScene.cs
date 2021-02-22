using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace InteractionFramework.Editor
{
    [InitializeOnLoad]
    public class AutoSaveHierarchyScene
    {
        static bool IsAutoSave = false;
        static AutoSaveHierarchyScene()
        {
            EditorApplication.playModeStateChanged += SaveOnPlay;
        }

        private static void SaveOnPlay(PlayModeStateChange state)
        {
            if (!IsAutoSave)
            {
                return;
            }
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                Debug.Log("Exiting Edit Mode,Auto Save...");
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        }
    }
}