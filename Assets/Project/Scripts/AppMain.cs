using System;
using System.Collections;
using System.Collections.Generic;
using InteractionFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace InteractionFramework.Runtime
{
    public class AppMain : MonoBehaviour
    {
        private void Awake()
        {
            InitServiceModule();
            InitBusinessModule();
        }

        private void InitServiceModule()
        {
            try
            {
                //本地持久化 将本地持久化文件放到StreamingAssets
                IniStorage.mINIFileName = Application.streamingAssetsPath + "/config.ini";
                //或者放到沙盒路径
                //IniStorage.mINIFileName = Application.persistentDataPath + "/config.ini";

                //初始化模块命名空间
                string namespacepath = "InteractionFramework.Runtime";
                ModuleManager.Instance.Init(namespacepath);


                //初始化多线程跨线程访问主线程工具
                UnityMainThreadDispatcher.InitSingletion();

                //启动摄像头
                //CameraModule.Instance.InitDevice(0,640,480);

                //消息中心
                MessageManager.InitSingleton();
            }
            catch (Exception ex)
            {
                Debug.LogError("InitServiceModule ERROR : " + ex);
                throw;
            }
        }

        private void InitBusinessModule()
        {
            try
            {
                //创建设置模块并初始化
                ConfigModule configModule = (ConfigModule)ModuleManager.Instance.CreateModule(ModuleDef.ConfigModule);
                configModule.InitResolution();
                configModule.InitMonitor();
                configModule.InitFrameRate();

                //图片处理工具
                TextureKit.InitSingletion();

                //二维码工具
                //ModuleManager.Instance.CreateModule(ModuleDef.QRCodeModule);
            }
            catch (Exception ex)
            {
                Debug.LogError("InitBusinessModule ERROR : " + ex);
                throw;
            }
        }
    }
}