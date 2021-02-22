using UnityEngine;
using System.Collections;

namespace InteractionFramework.Runtime.Demo
{
    public class MonoSingletionOne : MonoSingletion<MonoSingletionOne>
    {
        public void TestFunction()
        {
            Debug.Log("MonoSingletionOne");
        }
    }
}