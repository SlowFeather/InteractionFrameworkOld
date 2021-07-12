using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class UDPServerModule : BusinessModule
    {

        //以下默认都是私有的成员 
        Socket socket;                    //目标socket 
        EndPoint clientEnd;                 //上一个发消息的客户端 
        List<EndPoint> clientEnds;          //存储所有的UDP连接
        IPEndPoint ipEnd;                     //侦听端口 
        string recvStr;                   //接收的字符串 
        string sendStr;                   //发送的字符串 
        byte[] recvData = new byte[2048]; //接收的数据，必须为字节 
        byte[] sendData = new byte[2048]; //发送的数据，必须为字节 
        int recvLen;                   //接收的数据长度 
        Thread connectThread;                 //连接线程

        /// <summary>
        /// 收到消息回调
        /// </summary>
        public Action<string> ReceiveMessageAction;
        /// <summary>
        /// 收到消息回调
        /// </summary>
        public Action<byte[]> ReceiveRawMessageAction;

        //初始化
        public void InitSocket()
        {
            //定义侦听端口,侦听任何IP，切记：端口是自己定义的
            ipEnd = new IPEndPoint(IPAddress.Any, 9999);
            //定义套接字类型,在主线程中定义
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //服务端需要绑定ip
            socket.Bind(ipEnd);
            //定义客户端
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            clientEnd = (EndPoint)sender;
            //初始化客户端列表
            clientEnds = new List<EndPoint>();
            Debug.Log("server : waiting for UDP dgram");
            //开启一个线程连接，必须的，否则主线程卡死
            connectThread = new Thread(new ThreadStart(SocketReceiveAgreement));
            connectThread.Start();
        }

        public void SocketSend(string sendStr)
        {
            //清空发送缓存
            sendData = new byte[1024];
            //数据类型转换
            sendData = Encoding.UTF8.GetBytes(sendStr);
            //发送给所有客户端
            for (int i = 0; i < clientEnds.Count; i++)
            {
                socket.SendTo(sendData, clientEnds[i]);
                return;
            }
            Debug.LogWarning("server : not found client");
        }
        public void SocketSend(byte[] sendBytes)
        {
            //发送给所有客户端
            for (int i = 0; i < clientEnds.Count; i++)
            {
                socket.SendTo(sendBytes, clientEnds[i]);
                return;
            }
            Debug.LogWarning("server : not found client");
        }

        //检查是否在客户端列表中
        void CheckInList(EndPoint clientEnd)
        {
            if (recvData.Length > 0)
            {
                bool inList = false;
                for (int i = 0; i < clientEnds.Count; i++)
                {
                    //判断是否为相同连接
                    if (clientEnds[i].ToString() == clientEnd.ToString())
                    {
                        inList = true;
                    }
                }
                //如果这个连接不在列表中则添加到列表中
                if (!inList)
                {
                    clientEnds.Add(clientEnd);
                    Debug.Log("server : Add In ClientList,ListCount " + clientEnds.Count);
                }
            }
        }
        /// <summary>
        /// 根据协议接收数据
        /// </summary>
        void SocketReceiveAgreement()
        {
            //进入接收循环
            while (true)
            {
                //对data清零
                recvData = new byte[1024];
                //获取客户端，获取客户端数据，用引用给客户端赋值
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);

                Array.Resize(ref recvData, recvLen);

                //判断消息长度
                if (recvData.Length > 0)
                {
                    CheckInList(clientEnd);
                }

                byte[] vs = recvData;
                Debug.Log(vs[0] + vs[1] + vs[2]);

                UnityMainThreadDispatcher.Instance.Enqueue(() => {
                    ReceiveRawMessageAction?.Invoke(vs);
                });
                //Debug.Log("server : message from " + clientEnd.ToString()); //打印客户端信息
                //输出接收到的数据
                recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);

                //这里可能需要主线程
                UnityMainThreadDispatcher.Instance.Enqueue(() => {
                    ReceiveMessageAction?.Invoke(recvStr);
                    ReceiveRawMessageAction?.Invoke(recvData);
                });
            }
        }

        //测试服务器接收图片
        void SocketReceive()
        {
            //进入接收循环
            while (true)
            {
                //对data清零
                recvData = new byte[2048];
                //获取客户端，获取客户端数据，用引用给客户端赋值
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                //判断消息长度
                if (recvData.Length > 0)
                {
                    CheckInList(clientEnd);
                }
                
                //Debug.Log("server : message from " + clientEnd.ToString()); //打印客户端信息
                //输出接收到的数据
                recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);
                if (recvStr == "ImgEnd")
                {
                    //图片接收完成
                    CheckPackage();
                    return;
                }
                Debug.Log(recvData.Length);
                //if (recvData.Length > 18) //图片包头为29个字节
                if (recvStr.Length > 18) //图片包头为29个字节
                {
                    //合并发来的图片
                    ConmbineString(recvStr);
                }
                //这里可能需要主线程
                UnityMainThreadDispatcher.Instance.Enqueue(() =>{ 
                    ReceiveMessageAction?.Invoke(recvStr);
                });
            }
        }

        string newConbineStr="";
        void CheckPackage()
        {
            Debug.Log("接收成功");
            
            string dicStr = "";
            newConbineStr="";
            for (int i = 0; i < newImageDic.Count; i++)
            {
                
                if (newImageDic.TryGetValue(i, out dicStr))
                {
                    newConbineStr = newConbineStr + dicStr;
                }
            }
            newImageDic.Clear();

            //保存到本地
            ParseBYTeArr(newConbineStr);
            
        }

        //存储的字典
        Dictionary<int, string> newImageDic = new Dictionary<int, string>();
        //将分包发来的消息合成一个包
        void ConmbineString(string perStr)
        {
            //0.图片名字（21字节）--1.包的长度（1000为起始点，4字节）--2.包的下标（1000为起始点4个字节）--3.包的内容
            //分割字符串 "_"
            string[] strs = perStr.Split('_');
            //名字
            string newImageName = strs[0];
            int newImageCount = int.Parse(strs[1]) - 1000;
            int newStrIndex = int.Parse(strs[2]) - 1000;
            string newImageMessage = strs[3];
            newImageDic.Add(newStrIndex, newImageMessage);
        }


        /// <summary>
        /// 发来的字节包括：图片的字节长度（前四个字节）和图片字节
        /// 得到发来的字节中图片字节长度和图片字节数组
        /// </summary>
        void ParseBYTeArr(string receStr)
        {
            byte[] bytes = Convert.FromBase64String(receStr);



            //File.WriteAllBytes(filename, bytes);

            using (System.IO.MemoryStream mem = new MemoryStream(bytes))
            {
                string timestamp = GetTimeStamp().ToString();
                string filename = Application.streamingAssetsPath+"/UDPPhoto/" + timestamp + "UDP.jpg";//把接收到的UDP图片存到本地资源文件夹下

                //string NewFileJpg = Server.MapPath("~/allapi/" + ImageFileUrl) + NewFilethumbName; //保存路径
                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

            //Texture2D tex2D = new Texture2D(100, 100);
            //tex2D.LoadImage(bytes);

            //if (UDPserverEvent != null)
            //{
            //    UDPserverEvent(tex2D);
            //}
        }
        //连接关闭
        void SocketQuit()
        {
            //关闭线程
            if (connectThread != null)
            {
                connectThread.Interrupt();
                connectThread.Abort();
            }
            //最后关闭socket
            if (socket != null)
                socket.Close();
            Debug.Log("disconnect");
        }

        void OnApplicationQuit()
        {
            SocketQuit();
        }


        public static long GetTimeStamp(bool bflag = true)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ret;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds);
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret;
        }
    }
}