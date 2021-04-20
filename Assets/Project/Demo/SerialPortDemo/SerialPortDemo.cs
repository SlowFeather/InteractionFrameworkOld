using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class SerialPortDemo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SerialPortManager.InitSingletion();
        }

    }
}