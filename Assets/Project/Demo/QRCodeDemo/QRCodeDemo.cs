using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InteractionFramework.Runtime.Demo
{
    public class QRCodeDemo : MonoBehaviour
    {
        /// <summary>
        /// QRCode模块
        /// </summary>
        QRCodeModule qRCodeModule;

        /// <summary>
        /// 用来展示二维码
        /// </summary>
        public RawImage qrCodeTexture;

        /// <summary>
        /// 二维码内容
        /// </summary>
        public Text contentTxt;

        /// <summary>
        /// 相机图
        /// </summary>
        public RawImage camTexture;

        
        // Start is called before the first frame update
        void Start()
        {
            //初始化模块命名空间
            string namespacepath = "InteractionFramework.Runtime";
            ModuleManager.Instance.Init(namespacepath);

            //初始化多线程跨线程访问主线程工具
            UnityMainThreadDispatcher.InitSingletion();

            //启动摄像头
            CameraModule.Instance.InitDevice(0,640,480);

            if (CameraModule.Instance.webCamTexture!=null)
            {
                //拿到摄像头的图像
                camTexture.texture = CameraModule.Instance.webCamTexture;
            }

            //创建生成二维码工具模块
            qRCodeModule = (QRCodeModule)ModuleManager.Instance.CreateModule(ModuleDef.QRCodeModule);
            //添加生成结束监听
            qRCodeModule.CreateQRCodedEvent += CreateQRCodedEventHandler;
            //开始生成
            qRCodeModule.CreateQRCode("Test",512,512);

        }
        /// <summary>
        /// 二维码生成完毕监听 
        /// </summary>
        /// <param name="texture2d"></param>
        private void CreateQRCodedEventHandler(Texture2D texture2d)
        {
            qRCodeModule.CreateQRCodedEvent -= CreateQRCodedEventHandler;
            qrCodeTexture.texture = texture2d;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //让qrcode模块拿到摄像头的画面
                qRCodeModule.GetCameraTexture();
                //扫描二维码
                string s=qRCodeModule.ScanQRCode();
                if (s!=null)
                {
                    contentTxt.text = s;
                }
                else
                {
                    contentTxt.text = "没扫到";
                }
            }
        }


    }
}