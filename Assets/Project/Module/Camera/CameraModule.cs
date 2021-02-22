using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class CameraModule : ServiceModule<CameraModule>
    {
        public WebCamTexture webCamTexture;//摄像机映射纹理
        /// <summary>
        /// 开启摄像机和准备工作
        /// </summary>
        /// <param name="cameraIndex">设备索引号0,1,2</param>
        /// <param name="width">宽（像素）</param>
        /// <param name="height">高（像素）</param>
        public void InitDevice(int cameraIndex,int width,int height)
        {
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                //1、获取所有摄像机硬件
                WebCamDevice[] devices = WebCamTexture.devices;
                if (devices.Length == 0)
                {
                    Debug.LogError("No Camera");
                    return;
                }
                if (devices.Length < cameraIndex + 1)
                {
                    Debug.LogError("Camera Index ERROR");
                }
                //2、获取第一个摄像机硬件的名称
                string devicesName = devices[cameraIndex].name;//手机后置摄像机
                //3、创建实例化一个摄像机显示区域
                webCamTexture = new WebCamTexture(devicesName, width, height);
                //4、显示的图片信息
                //cameraTexture.texture = webCamTexture;
                //5、打开摄像机运行识别
                webCamTexture.Play();
            }
            else
            {
                Debug.LogError("Not Allow Use Camera");
            }


            
        }
        /// <summary>
        /// 停止当前摄像头工作
        /// </summary>
        public void StopDevice()
        {
            if (webCamTexture.isPlaying)
            {
                webCamTexture.Stop();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (webCamTexture!=null)
            {
                webCamTexture.Stop();
            }
            
        }
    }
}