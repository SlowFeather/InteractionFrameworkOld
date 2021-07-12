using InteractionFramework.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
namespace InteractionFramework.Runtime
{

    public class UDPClientModule :BusinessModule
    {
        Socket socket; //目标socket
        EndPoint serverEnd; //服务端
        IPEndPoint ipEnd; //服务端端口
        string recvStr; //接收的字符串
        string sendStr; //发送的字符串
        byte[] recvData = new byte[1024]; //接收的数据，必须为字节
        byte[] sendData = new byte[1024]; //发送的数据，必须为字节
        int recvLen = 0; //接收的数据长度
        Thread connectThread; //连接线程

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
            //定义连接的服务器ip和端口，可以是本机ip，局域网，互联网，IP和端口是需要自己设置的
            ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            //定义套接字类型,在主线程中定义
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //定义服务端,全网段皆可
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            serverEnd = (EndPoint)sender;
            //socket.Bind(ipEnd);
            Debug.Log("client : wait send UDP dgram");
            //建立初始连接，这句非常重要，第一次连接初始化了serverEnd后面才能收到消息
            SocketSend("Msg");
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
            //发送给指定服务端
            socket.SendTo(sendData, ipEnd);
        }

        public void SocketSend(byte[] bytes)
        {
            //发送给指定服务端
            socket.SendTo(bytes, ipEnd);
        }

        //客户端接收
        void SocketReceiveAgreement()
        {
            //进入接收循环
            while (true)
            {
                //对data清零
                recvData = new byte[1024];
                //获取客户端，获取服务端端数据，用引用给服务端赋值，实际上服务端已经定义好并不需要赋值
                recvLen = socket.ReceiveFrom(recvData, ref serverEnd);

                //Debug.Log("client : 信息来自: " + serverEnd.ToString()); //打印服务端信息//输出接收到的数据
                recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);


                //这里可能需要主线程
                UnityMainThreadDispatcher.Instance.Enqueue(() => {
                    ReceiveMessageAction?.Invoke(recvStr);
                    ReceiveRawMessageAction?.Invoke(recvData);
                });
            }
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
        }

        void OnApplicationQuit()
        {
            SocketQuit();
        }
    }
}