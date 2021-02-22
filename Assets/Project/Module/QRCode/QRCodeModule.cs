using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;

namespace InteractionFramework.Runtime
{
    public class QRCodeOption
    {
        public string url;
        public Vector2Int size;
    }

    public class QRCodeModule : BusinessModule
    {
        public override void Create(object args = null)
        {
            base.Create(args);
            
            //初始化的时候就要把相关的辅助模块准备好
            QRCodeHelper.InitSingletion();
            QRCodeHelper.CheckInstance();
        }

        /// <summary>
        /// 摄像头Texture
        /// </summary>
        private WebCamTexture cameraTexture;
        /// <summary>
        /// 创建QRCode完成的回调
        /// </summary>
        public Action<Texture2D> CreateQRCodedEvent;

        /// <summary>
        /// 获取Camera的Texture
        /// </summary>
        public void GetCameraTexture()
        {
            if (CameraModule.Instance.webCamTexture != null)
            {
                if (CameraModule.Instance.webCamTexture.isPlaying)
                {
                    cameraTexture = CameraModule.Instance.webCamTexture;
                }
                else
                {
                    Debug.LogError("The Camera Is Not Running");
                }
            }
            else
            {
                Debug.LogError("First, Turn On The Camera \"CameraModule.Instance.InitDevice\"");
            }
        }

        /// <summary>
        /// 创建QRCode
        /// </summary>
        public void CreateQRCode(string url,int width,int height)
        {
            QRCodeHelper.Instance.CreateQRCodeEndEvent += CreateQRCodedHandler;
            QRCodeHelper.Instance.CreateQRCode(url,width,height);
        }

        private void CreateQRCodedHandler(Texture2D texture)
        {
            QRCodeHelper.Instance.CreateQRCodeEndEvent -= CreateQRCodedHandler;
            CreateQRCodedEvent?.Invoke(texture);
        }

        /// <summary>
        /// 扫描QRCode
        /// </summary>
        /// <returns></returns>
        public string ScanQRCode()
        {
            return QRCodeHelper.Instance.ScanQRCode(cameraTexture.GetPixels32(), cameraTexture.width, cameraTexture.height);
        }
            
    }
}