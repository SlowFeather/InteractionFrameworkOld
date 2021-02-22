using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class MessageManager:Singleton<MessageManager>
    {

        //因为客户端和服务器之间的消息都是根据不同的传输协议进行字节数组的传输,改成buffer即可
        public delegate void OnActinHandler(object a);

        //委托字典
        Dictionary<ushort, List<OnActinHandler>> dic = new Dictionary<ushort, List<OnActinHandler>>();

        /// <summary>
        /// 添加监听 [给监听者使用，想监听了就添加，不需要监听的就不用管]
        /// </summary>
        /// <param name="protoID"></param>
        /// <param name="handler"></param>
        public void AddEventListener(ushort protoID, OnActinHandler handler)
        {
            if (dic.ContainsKey(protoID))
            {
                dic[protoID].Add(handler);
            }
            else
            {
                List<OnActinHandler> lsHandler = new List<OnActinHandler>();
                lsHandler.Add(handler);
                dic[protoID] = lsHandler;
            }
        }
        /// <summary>
        /// 移除监听  [给监听者使用，如果有监听某条消息必须有移除，否则死人还能收到消息]
        /// </summary>
        /// <param name="protoID"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(ushort protoID, OnActinHandler handler)
        {
            if (dic.ContainsKey(protoID))
            {
                List<OnActinHandler> lsHandler = dic[protoID];
                lsHandler.Remove(handler);
                if (lsHandler.Count == 0)
                {
                    dic.Remove(protoID);
                }
            }
            else
            {
                Debug.LogWarning("No protoID");
            }

        }
        /// <summary>
        /// 派发消息 【给发布者使用,不监听的人我就不发给他，谁监听谁能收到】
        /// </summary>
        /// <param name="protoID"></param>
        /// <param name="arg"></param>
        public void Dispatch(ushort protoID, object buffer)
        {
            if (dic.ContainsKey(protoID))
            {
                //先根据id将list拿到
                List<OnActinHandler> lsHandler = dic[protoID];
                if (lsHandler != null && lsHandler.Count > 0)
                {
                    for (int i = 0; i < lsHandler.Count; i++)
                    {
                        //判断空执行
                        if (lsHandler[i] != null)
                        {
                            lsHandler[i](buffer);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("No protoID");
            }
        }

        public void RemoveAllEventListener(ushort protoID)
        {
            if (dic.ContainsKey(protoID))
            {
                dic.Remove(protoID);
            }
            else
            {
                Debug.LogWarning("No protoID");
            }
        }
    }
}