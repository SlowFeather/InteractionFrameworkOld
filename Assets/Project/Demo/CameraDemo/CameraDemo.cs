using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InteractionFramework.Runtime.Demo
{
    public class CameraDemo : MonoBehaviour
    {
        public RawImage camTexture;

        public int cameraIndex=0;
        public int cameraWidth = 640;
        public int cameraHeight = 480;

        void Start()
        {
            //启动摄像头
            CameraModule.Instance.InitDevice(0, 640, 480);

            if (CameraModule.Instance.webCamTexture != null)
            {
                //拿到摄像头的图像
                camTexture.texture = CameraModule.Instance.webCamTexture;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CameraModule.Instance.webCamTexture.isPlaying)
                {
                    CameraModule.Instance.StopDevice();
                }
                else
                {
                    CameraModule.Instance.InitDevice(0, 640, 480);
                    camTexture.texture = CameraModule.Instance.webCamTexture;
                }
                
            }
        }
    }
}