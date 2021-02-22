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
            MessageManager.Instance.Dispatch(10001, "Hello");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}