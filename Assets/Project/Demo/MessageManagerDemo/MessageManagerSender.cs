using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class MessageManagerSender : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //发送消息
            MessageManager.Instance.Dispatch(10001, "Hello");
        }
    }
}