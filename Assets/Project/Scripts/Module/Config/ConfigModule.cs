using InteractionFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class ConfigModule : BusinessModule
    {
        /// <summary>
        /// 初始化分辨率
        /// </summary>
        public void InitResolution()
        {
            int screenWidth = IniStorage.GetInt("ScreenWidth");
            int screenHeight = IniStorage.GetInt("ScreenHeight");
            bool isFullScreen = IniStorage.GetBool("FullScreen");
            //设置分辨率和是否全屏
            Screen.SetResolution(screenWidth, screenHeight, isFullScreen);
            Debug.Log("Resolution : " + screenWidth +"-"+ screenHeight);
            Debug.Log("FullScreen : " + isFullScreen);

            //设置渲染等级
            //QualitySettings.SetQualityLevel(1, true); 
        }
        /// <summary>
        /// 初始化播放屏幕
        /// 设置完屏幕需要重启生效
        /// </summary>
        public void InitMonitor()
        {
            int selectMonitor = IniStorage.GetInt("SelectMonitor");
            //设置播放屏幕 0为一号屏 1为二号屏
            PlayerPrefs.SetInt("UnitySelectMonitor", selectMonitor);
            Debug.Log("Monitor : " + selectMonitor);

        }
        /// <summary>
        /// 设置帧率
        /// </summary>
        public void InitFrameRate()
        {
            int frameRate = IniStorage.GetInt("FrameRate");
            //设置帧率
            Application.targetFrameRate = frameRate;
            Debug.Log("FrameRate : " + frameRate);
        }
    }
}