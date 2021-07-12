using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo{


    public class UDPDemo : MonoBehaviour
    {
        /// <summary>
        /// UDP Server Module
        /// </summary>
        UDPServerModule uDPServerModule;
        /// <summary>
        /// UDP Client Module
        /// </summary>
        UDPClientModule uDPClientModule;

        // Start is called before the first frame update
        void Start()
        {
            //初始化模块命名空间
            string namespacepath = "InteractionFramework.Runtime";
            ModuleManager.Instance.Init(namespacepath);
            //初始化配置文件
            IniStorage.mINIFileName = Application.streamingAssetsPath + "/IFConfig/config.ini";
            //初始化多线程跨线程访问主线程工具
            UnityMainThreadDispatcher.InitSingletion();
            UnityMainThreadDispatcher.CheckInstance();


            //初始化udp服务器
            uDPServerModule= (UDPServerModule)ModuleManager.Instance.CreateModule(ModuleDef.UDPServerModule);
            uDPServerModule.ReceiveMessageAction += ServerReceiveMessageHandler;

            //初始化udp客户端
            uDPClientModule = (UDPClientModule)ModuleManager.Instance.CreateModule(ModuleDef.UDPClientModule);
            uDPClientModule.ReceiveMessageAction+= ClientReceiveMessageHandler;


            uDPServerModule.InitSocket();
            uDPClientModule.InitSocket();
        }

        private void ClientReceiveMessageHandler(string msg)
        {
            Debug.Log("客户端收到消息：" + msg);
        }

        void ServerReceiveMessageHandler(string msg)
        {
            Debug.Log("服务器收到消息：" + msg);
        }

        /// <summary>
        /// 图片转成base64后的字典
        /// </summary>
        Dictionary<int, string> UDPStringDic = new Dictionary<int, string>();

        void ClientSendImgToServer()
        {
            string path = Application.dataPath + "/Project/Demo/UDPDemo/222.jpg";
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                string fileName = System.IO.Path.GetFileName(path);
                string fileExtension = System.IO.Path.GetExtension(path);
                byte[] imagebytes = new byte[fs.Length];
                fs.Read(imagebytes, 0, imagebytes.Length);
                string base64Str = Convert.ToBase64String(imagebytes);
                //这里要把图片拆成一个个小包
                UDPSplit(base64Str);
                
                for (int i = 0; i < num - 1000; i++)
                {
                    if (UDPStringDic.TryGetValue(i, out newImageString))
                    {
                        uDPClientModule.SocketSend(newImageString);
                    }
                }
            }
            //发送完成后发一条信息给服务端
            uDPClientModule.SocketSend("ImgEnd");
        }
        int num;
        string newImageString;
        void UDPSplit(string str)
        {
            int index = 0;
            int maxIndex = 1000;
            int stringTag = 1000;
            //清空字典
            UDPStringDic.Clear();
            //长度/1000 + 1+1000
            num = (str.Length / 1000) + 1 + 1000;   //将数字变成四位数的，三个字节
                                                    //  print(num-1000);
            for (int i = 0; i < num - 1000; i++)
            {
                if (maxIndex > str.Length - index)
                {
                    maxIndex = str.Length - index;
                }
                string newstr = "1551683020" + "_" + num + "_" + stringTag + "_" + str.Substring(index, maxIndex); //包名，包长，包的顺序号，包的内容


                UDPStringDic.Add(stringTag - 1000, newstr);
                stringTag++;
                index += 1000;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                uDPClientModule.SocketSend("Hello Server");
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                uDPServerModule.SocketSend("Hello Client");
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                ClientSendImgToServer();
            }

        }
    }
}