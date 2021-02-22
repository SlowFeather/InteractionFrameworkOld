using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class ModuleDemo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            string namespacepath = "InteractionFramework.Runtime.Demo";
            ModuleManager.Instance.Init(namespacepath);
            ModuleManager.Instance.CreateModule("TestModuleOne");
            ModuleManager.Instance.CreateModule("TestModuleTwo");

            ModuleManager.Instance.SendMessage("TestModuleOne", "HelloOne", "MsgContent");
            ModuleManager.Instance.SendMessage("TestModuleTwo", "HelloTwo", "MsgContent");

            TestModuleOne module1 = ModuleManager.Instance.GetModule<TestModuleOne>();
            TestModuleOne module2 = ModuleManager.Instance.GetModule("TestModuleOne") as TestModuleOne;
            Debug.Log(module1.Name);
            Debug.Log(module2.Name);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

