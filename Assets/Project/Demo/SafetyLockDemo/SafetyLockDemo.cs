using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class SafetyLockDemo : MonoBehaviour
    {
        SafetyLockModule safetyLockModule;
        // Start is called before the first frame update
        void Start()
        {
            //初始化模块命名空间
            string namespacepath = "InteractionFramework.Runtime";
            ModuleManager.Instance.Init(namespacepath);

            
            safetyLockModule = (SafetyLockModule)ModuleManager.Instance.CreateModule(ModuleDef.SafetyLockModule);

            safetyLockModule.availableTime = 1;
            safetyLockModule.ExpireEvent += ExpireEventHandler;

            safetyLockModule.Init();

        }

        private void ExpireEventHandler(bool isExpire)
        {
            if (isExpire)
            {
                Debug.Log("过期了");

            }
            else
            {
                Debug.Log("没过期");

            }
        }
    }
}

