using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class MultiThreadDemo : MonoBehaviour
    {
        /// <summary>
        /// 测试线程
        /// </summary>
        Thread threadOne;


        void Start()
        {
            //初始化多线程访问主线程工具
            UnityMainThreadDispatcher.InitSingletion();
            UnityMainThreadDispatcher.CheckInstance();

            //新建一个线程
            threadOne = new Thread(SubThread);
            threadOne.Start();
        }

        /// <summary>
        /// Thread方式
        /// </summary>
        private void SubThread()
        {
            for (int i = 0; i < 10; i++)
            {
                //第一种访问主线程
                UnityMainThreadDispatcher.Instance.Enqueue(() => {
                    Debug.Log(" Log1 : "+"This is executed from the main thread");
                    this.transform.position = Vector3.one * i;
                });
                //第二种访问主线程
                UnityMainThreadDispatcher.Instance.Enqueue(ThisWillBeExecutedOnTheMainThread());
                Thread.Sleep(1000);
            }
            
        }


        /// <summary>
        /// IEnumerator方式
        /// </summary>
        /// <returns></returns>
        public IEnumerator ThisWillBeExecutedOnTheMainThread()
        {
            Debug.Log(" Log2 : "+"This is executed from the main thread");
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles+Vector3.one);
            yield return null;
        }


        private void OnDestroy()
        {
            if (threadOne.IsAlive && threadOne != null)
            {
                threadOne.Abort();
                threadOne = null;
            }

        }
    }
}