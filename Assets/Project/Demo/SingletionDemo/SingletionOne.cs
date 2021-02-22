using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class SingletionOne : Singleton<SingletionOne>
    {
        public void TestFunction()
        {
            Debug.Log("SingletionOne");
        }
    }
}