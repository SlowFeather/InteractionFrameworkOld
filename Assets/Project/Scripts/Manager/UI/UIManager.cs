using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class UIManager : MonoSingletion<UIManager>
    {
        /// <summary>
        /// UI路径
        /// </summary>
        public string uiPath = "Prefabs/UI/";

        /// <summary>
        /// UI根物体一般为Canvas
        /// </summary>
        public GameObject uiRoot;

        /// <summary>
        /// 存储所有UI的字典
        /// </summary>
        public Dictionary<string, UIPage> uiDictionary;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="uiroot"></param>
        public void InitUIManager(GameObject uiroot)
        {
            uiRoot = uiroot;
            uiDictionary = new Dictionary<string, UIPage>();
        }


        public T GetPage<T>(string ui) where T : UIPage
        {
            foreach (var item in uiDictionary)
            {
                if (item.Key == ui)
                {
                    //显示UI
                    item.Value.gameObject.SetActive(true);
                    return item.Value as T;
                }
            }
            Debug.LogError("UIModule Not Find " + ui);
            return null;
        }

        /// <summary>
        /// 打开页面 
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="arg"></param>
        public void OpenPage(string ui,object arg = null)
        {
            foreach (var item in uiDictionary)
            {
                if (item.Key==ui)
                {
                    //显示UI
                    item.Value.Open(arg);
                    return;
                }
            }
            //生成UI并添加到字典中
            GameObject gameObject = GameObject.Instantiate(Resources.Load<GameObject>(uiPath + ui), uiRoot.transform);
            UIPage uIPage = gameObject.GetComponent<UIPage>();
            uiDictionary.Add(ui, uIPage);
            gameObject.SetActive(true);
            uIPage.Initialize();
            uIPage.Open(arg);

        }
        //public void JumpPage()
        //{

        //}

        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <param name="ui"></param>
        public void ClosePage(string ui)
        {
            foreach (var item in uiDictionary)
            {
                if (item.Key == ui)
                {
                    //显示UI
                    item.Value.Close();
                    return;
                }
            }
            Debug.LogError("UIModule Not Find This UI");
            
        }

        public void RemovePage(string ui)
        {
            foreach (var item in uiDictionary)
            {
                if (item.Key == ui)
                {
                    //移除UI
                    if (item.Value.gameObject.activeInHierarchy)
                    {
                        Debug.LogError("UIModule This UI is active,Plase Close ...");
                        return;
                    }
                    else
                    {
                        GameObject desUI = item.Value.gameObject;
                        uiDictionary.Remove(ui);
                        GameObject.Destroy(desUI);
                    }

                    return;
                }
                
            }
            Debug.LogError("UIModule Not Find This UI");
        }

        
    }
}