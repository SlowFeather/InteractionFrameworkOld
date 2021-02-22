using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractionFramework.Runtime.Demo
{
    public class MonoSingletionTwo :MonoSingletion<MonoSingletionTwo>
    {
        public void TestFunction()
        {
            Debug.Log("MonoSingletionTwo");
        }
    }
}