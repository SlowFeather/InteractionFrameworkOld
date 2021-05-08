using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class UIDemo : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            //检查单例
            UIManager.InitSingletion();
            UIManager.CheckInstance();
            //设置预制体路径
            UIManager.Instance.uiPath = "Prefabs/UIDemo/";
            //找到UI根物体
            GameObject uiRoot = GameObject.Find("Canvas");

            //初始化
            UIManager.Instance.InitUIManager(uiRoot);
            //打开界面
            UIManager.Instance.OpenPage(TestUIDef.TestUI);

            UIManager.Instance.OpenPage(TestUIDef.Test2UI);
            Timer.Register(2f, () => {
                //拿到界面
                UIManager.Instance.GetPage<TestUI>(TestUIDef.TestUI).Hello();
                //关闭界面
                UIManager.Instance.ClosePage(TestUIDef.TestUI);
                //移除界面
                //UIManager.Instance.RemovePage(TestUIDef.TestUI);

            });
            Timer.Register(4f, () => {

                //关闭界面
                UIManager.Instance.ClosePage(TestUIDef.Test2UI);
                UIManager.Instance.OpenPage(TestUIDef.TestUI);

            });

        }


    }
}