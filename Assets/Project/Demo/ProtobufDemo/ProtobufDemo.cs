using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtobufMsg;
namespace InteractionFramework.Runtime.Demo
{
    public class ProtobufDemo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //初始化模块命名空间
            string namespacepath = "InteractionFramework.Runtime";
            ModuleManager.Instance.Init(namespacepath);

            //监听网络消息
            MessageManager.Instance.AddEventListener(Opcode.CaterpillarPullRequest, CaterpillarPullRequestHandler);
            MessageManager.Instance.AddEventListener(Opcode.CaterpillarPullResponse, CaterpillarPullResponseHandler);
        }
        private void CaterpillarPullResponseHandler(object a)
        {
            CaterpillarPullResponse caterpillarPullResponse = a as CaterpillarPullResponse;
            Debug.Log("CaterpillarPullResponse : " + caterpillarPullResponse.code + "-" + caterpillarPullResponse.message);
        }

        private void CaterpillarPullRequestHandler(object a)
        {
            CaterpillarPullRequest caterpillarPullRequest = a as CaterpillarPullRequest;
            Debug.Log("CaterpillarPullRequest : " + caterpillarPullRequest.clientname + "-" + caterpillarPullRequest.shapes);
        }
        byte[] mmessageBytes;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //第一条消息
                CaterpillarPullRequest caterpillarPullRequest = new CaterpillarPullRequest();
                caterpillarPullRequest.clientname = "虫虫客户端1";
                caterpillarPullRequest.shapes = "1/2/3/4/5/6/7/8/11/12/13";
                caterpillarPullRequest.imgstyles = "1/2/3/4/5/6/7/8/11/12/13";

                //序列化第一条消息
                byte[] caterpillarPullRequestBytes = ProtobufHelper.Serialize<CaterpillarPullRequest>(caterpillarPullRequest);
                //创建第一条消息的外层包
                mmessageBytes=Packet(Opcode.CaterpillarPullRequest, caterpillarPullRequestBytes);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                //第二条消息
                CaterpillarPullResponse caterpillarPullResponse = new CaterpillarPullResponse();
                caterpillarPullResponse.code = 1;
                caterpillarPullResponse.message = "AllRight";
                //序列化第一条消息
                byte[] caterpillarPullResponseBytes = ProtobufHelper.Serialize<CaterpillarPullResponse>(caterpillarPullResponse);
                //创建第二条消息的外层包
                mmessageBytes = Packet(Opcode.CaterpillarPullResponse, caterpillarPullResponseBytes);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                //解析消息
                UnPacket(mmessageBytes);
            }

        }


        /// <summary>
        /// 把消息打到外层包中
        /// </summary>
        /// <param name="msgid"></param>
        /// <param name="databytes"></param>
        public byte[] Packet(ushort msgid, byte[] databytes)
        {
            MessagePackage messagePackage = new MessagePackage();
            messagePackage.protoid = msgid;
            messagePackage.protodata = databytes;
            //序列化消息的外层包
            byte[] messagePackageBytes = ProtobufHelper.Serialize<MessagePackage>(messagePackage);
            return messagePackageBytes;
        }

        /// <summary>
        /// 把消息打从外层包中解开
        /// </summary>
        /// <param name="msgid"></param>
        /// <param name="databytes"></param>
        public void UnPacket(byte[] msgbytes)
        {
            MessagePackage msg = ProtobufHelper.Deserialize<MessagePackage>(msgbytes);
            switch (msg.protoid)
            {
                case Opcode.CaterpillarPullRequest: PackMessage<CaterpillarPullRequest>(msg.protoid, ProtobufHelper.Deserialize<CaterpillarPullRequest>(msg.protodata)); break;
                case Opcode.CaterpillarPullResponse: PackMessage<CaterpillarPullResponse>(msg.protoid, ProtobufHelper.Deserialize<CaterpillarPullResponse>(msg.protodata)); break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 将消息分发出去
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        public void PackMessage<T>(int id, T msg)
        {
            MessageManager.Instance.Dispatch((ushort)id, msg);
        }
    }
}