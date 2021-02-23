using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class MessageManagerReceiver : MonoBehaviour
    {
        private void Awake()
        {
            //添加监听
            MessageManager.Instance.AddEventListener(10001, ReceiveFunction);
        }

        /// <summary>
        /// 收到消息回调
        /// </summary>
        /// <param name="a"></param>
        private void ReceiveFunction(object a)
        {
            string msg = (string)a;
            Debug.Log(msg);
        }


        private void OnDestroy()
        {
            //移除监听
            MessageManager.Instance.RemoveEventListener(10001, ReceiveFunction);
        }
    }
}