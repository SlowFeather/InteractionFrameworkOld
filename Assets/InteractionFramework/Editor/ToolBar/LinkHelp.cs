using UnityEditor;
using UnityEngine;

namespace InteractionFramework.Editor
{
    /// <summary>
    /// 帮助相关的实用函数。
    /// </summary>
    public static class LinkHelp
    {
        [MenuItem("Interaction Framework/Bai du", false, 90)]
        public static void ShowDocumentation()
        {
            ShowHelp("https://www.baidu.com");
        }

        private static void ShowHelp(string uri)
        {
            Application.OpenURL(uri);
        }
    }
}
