using System;
using System.Collections;
using System.Collections.Generic;
using InteractionFramework.Runtime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace InteractionFramework.Runtime
{
    public class AppMain : MonoBehaviour
    {

        [ReadOnly]
        public string Interaction="Framework";
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
                IniStorage.mINIFileName = Application.streamingAssetsPath + "/IFConfig/config.ini";
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

                //UI相关
                UIManager.InitSingletion();
                UIManager.Instance.InitUIManager(GameObject.Find("UIRoot"));
                UIManager.Instance.uiPath = "Prefabs/UI/";
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

                //快捷键工具
                ShortcutKeyMangager.InitSingletion();

                //串口工具
                //SerialPortManager.InitSingletion();

                //创建加密模块
                SafetyLockModule safetyLockModule = (SafetyLockModule)ModuleManager.Instance.CreateModule(ModuleDef.SafetyLockModule);
                //设置一天过期 0则永不过期
                safetyLockModule.availableTime = 0;
                //safetyLockModule.availableTime = 1;
                //添加过没过期的监听
                safetyLockModule.ExpireEvent += ExpireEventHandler;
                //初始化加密模块
                safetyLockModule.Init();

            }
            catch (Exception ex)
            {
                Debug.LogError("InitBusinessModule ERROR : " + ex);
                throw;
            }
        }

        private void ExpireEventHandler(bool isExpire)
        {
            if (isExpire)
            {
                //Debug.Log("过期了");

            }
            else
            {
                //Debug.Log("没过期");

            }
        }
    }
}