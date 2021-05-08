using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo {
    public class ConfigDemo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            string namespacepath = "InteractionFramework.Runtime";
            ModuleManager.Instance.Init(namespacepath);
            //创建生成二维码工具模块
            ConfigModule configModule=(ConfigModule)ModuleManager.Instance.CreateModule(ModuleDef.ConfigModule);
            configModule.InitResolution();
            configModule.InitMonitor();
            configModule.InitFrameRate();
        }

    }
}
