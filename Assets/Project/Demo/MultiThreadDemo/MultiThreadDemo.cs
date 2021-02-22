using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class MultiThreadDemo : MonoBehaviour
    {
        Thread threadOne;

        // Start is called before the first frame update
        void Start()
        {
            //初始化管理器
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
                Debug.Log(Thread.CurrentThread.Name+" Log : " + i);
                //访问主线程
                UnityMainThreadDispatcher.Instance.Enqueue(() => {
                    Debug.Log("This is executed from the main thread");
                    this.transform.position = Vector3.one * i;
                });
                Thread.Sleep(1000);
            }
            ExampleMainThreadCall();
        }

        /// <summary>
        /// IEnumerator方式
        /// </summary>
        /// <returns></returns>
        public IEnumerator ThisWillBeExecutedOnTheMainThread()
        {
            Debug.Log("This is executed from the main thread");
            this.transform.position = Vector3.one * 20;
            yield return null;
        }
        public void ExampleMainThreadCall()
        {
            //访问主线程
            UnityMainThreadDispatcher.Instance.Enqueue(ThisWillBeExecutedOnTheMainThread());
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