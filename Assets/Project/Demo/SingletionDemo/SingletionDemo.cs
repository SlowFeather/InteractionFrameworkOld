using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class SingletionDemo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //MonoSingletion
            MonoSingletionOne.InitSingletion();
            MonoSingletionOne.CheckInstance();

            MonoSingletionOne.Instance.TestFunction();

            MonoSingletionTwo.InitSingletion();
            MonoSingletionTwo.CheckInstance();

            MonoSingletionTwo.Instance.TestFunction();

            //Singletion
            SingletionOne.Instance.TestFunction();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}