using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class ShortcutKeyMangager : MonoSingletion<ShortcutKeyMangager>
    {

        // Update is called once per frame
        void Update()
        {
            // 当按下 Q 键时，鼠标隐藏
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cursor.visible = false;
            }
            // 当按下 W 键时，鼠标显示
            if (Input.GetKeyDown(KeyCode.W))
            {
                Cursor.visible = true;
            }
            // 当按下 ESC 键时，退出
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}