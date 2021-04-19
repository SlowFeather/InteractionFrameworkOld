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
            //初始化ini读写模块
            IniStorage.mINIFileName = Application.streamingAssetsPath + "/IFConfig/config.ini";

            //创建模块
            safetyLockModule = (SafetyLockModule)ModuleManager.Instance.CreateModule(ModuleDef.SafetyLockModule);
            
            //设置一天过期 0则永不过期
            //safetyLockModule.availableTime = 0;
            safetyLockModule.availableTime = 1;

            //添加过没过期的监听
            safetyLockModule.ExpireEvent += ExpireEventHandler;
            //初始化
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

