using UnityEditor;
using UnityEngine;

namespace BgTools.PlayerPrefsEditor
{
    public class PlayerPrefsTestMenu
    {

        [MenuItem("Interaction Framework/PlayerPrefs/PlayerPrefs Test/AddEntry Strings")]
        public static void addTestValueString()
        {
            PlayerPrefs.SetString("String", "boing");
            PlayerPrefs.SetString("String2", "foo");
            PlayerPrefs.Save();
        }

        [MenuItem("Interaction Framework/PlayerPrefs/PlayerPrefs Test/AddEntry Int")]
        public static void addTestValueInt()
        {
            PlayerPrefs.SetInt("Int", 1234);
            PlayerPrefs.Save();
        }

        [MenuItem("Interaction Framework/PlayerPrefs/PlayerPrefs Test/AddEntry Float")]
        public static void addTestValueFloat()
        {
            PlayerPrefs.SetFloat("Float", 3.14f);
            PlayerPrefs.Save();
        }

        [MenuItem("Interaction Framework/PlayerPrefs/PlayerPrefs Test/DeleteAll")]
        public static void deleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}