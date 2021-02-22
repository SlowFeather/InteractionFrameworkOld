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
            MessageManager.Instance.AddEventListener(10001, ReceiveFunction);
        }

        private void ReceiveFunction(object a)
        {
            string msg = (string)a;
            Debug.Log(msg);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        
        private void OnDestroy()
        {
            MessageManager.Instance.RemoveEventListener(10001, ReceiveFunction);
        }
    }
}