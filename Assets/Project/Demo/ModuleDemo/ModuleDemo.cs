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
            //初始化Module
            string namespacepath = "InteractionFramework.Runtime.Demo";
            ModuleManager.Instance.Init(namespacepath);
            //创建Module
            ModuleManager.Instance.CreateModule("TestModuleOne");
            ModuleManager.Instance.CreateModule("TestModuleTwo");
            //给某个Module发消息
            ModuleManager.Instance.SendMessage("TestModuleOne", "HelloOne", "MsgContent");
            ModuleManager.Instance.SendMessage("TestModuleTwo", "HelloTwo", "MsgContent");
            //两种获取Module的方法
            TestModuleOne module1 = ModuleManager.Instance.GetModule<TestModuleOne>();
            TestModuleOne module2 = ModuleManager.Instance.GetModule("TestModuleOne") as TestModuleOne;
            //打印Module的名字
            Debug.Log(module1.Name);
            Debug.Log(module2.Name);

        }
    }
}

