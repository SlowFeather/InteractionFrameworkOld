using UnityEngine;
using System.Collections;
namespace InteractionFramework.Runtime
{
    //public class TestSingleton : MonoBehaviour
    //{
    //   public  static TestSingleton instance;
    //    private void Awake()
    //    {
    //        instance = this;
    //    }
    //}
    //上面这张写法也是单例，比较简单 但是要求比较特殊，这个脚本组件必须只能在此项目中唯一的物体上有，其他体身上不能有，像主摄像机 
    //【一定要确保这种写法的单例只能出现在场景中唯一的一个物体上】
    public class MonoSingletion<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static string MonoSingletionName = "[MonoSingletionRoot]";
        private static GameObject MonoSingletionRoot;
        private static T instance;

        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    return null;
                }
                if (MonoSingletionRoot == null)//如果是第一次调用单例类型就查找所有单例类的总结点
                {
                    MonoSingletionRoot = GameObject.Find(MonoSingletionName);
                    if (MonoSingletionRoot == null)//如果没有找到则创建一个所有继承MonoBehaviour单例类的节点
                    {
                        MonoSingletionRoot = new GameObject();
                        MonoSingletionRoot.name = MonoSingletionName;
                        DontDestroyOnLoad(MonoSingletionRoot);//防止被销毁
                    }
                }
                if (instance == null)//为空表示第一次获取当前单例类
                {
                    instance = MonoSingletionRoot.GetComponent<T>();
                    if (instance == null)//如果当前要调用的单例类不存在则添加一个
                    {
                        instance = MonoSingletionRoot.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        public static T InitSingletion()
        {
            return Instance;
        }

        public static void CheckInstance()
        {
            if (instance == null)
            {
                Debug.LogError("脚本类<" + typeof(T).Name + "> 无法直接实例化，因为它是一个单例!");
                throw new System.Exception("脚本类<" + typeof(T).Name + "> 无法直接实例化，因为它是一个单例!");
            }
        }

        public void OnDestroy()
        {
            _applicationIsQuitting = true;
            instance = null;
        }

    }
}