using UnityEngine;
using System.Threading;
using ZXing;
using ZXing.Common;
using System.Collections.Generic;
using System;
using System.Collections;
using InteractionFramework.Runtime;
namespace InteractionFramework.Runtime
{

    /// <summary>
    /// QRCode生成帮助类。需要依赖Mono因为要把Texture2D回调出去
    /// </summary>
    public class QRCodeHelper : MonoSingletion<QRCodeHelper>
    {
        /// <summary>
        /// 是否正在创建QRCode
        /// </summary>
        public bool CreateQRCodingFlag = false;
        /// <summary>
        /// 是否生成结束 用于Update判断
        /// </summary>
        public bool CreateQRCodeEnded = false;

        /// <summary>
        /// 创建完成的Texture
        /// </summary>
        public Texture2D CreateQRCodeTexture2D;
        /// <summary>
        /// 创建结束Event
        /// </summary>
        public Action<Texture2D> CreateQRCodeEndEvent;
        public void CreateQRCode(string qrcodeurl,int width=512,int height=512)
        {
            if (CreateQRCodingFlag)
            {
                //当前正在生成
                return;
            }
            CreateQRCodingFlag = true;
            CreateQRCodeEnded = false;
            Thread task = new Thread(new ParameterizedThreadStart(CreateQRCodeWorker));
            task.Start(new QRCodeOption() { url = qrcodeurl, size = new Vector2Int() { x = width, y = height } });
        }

        private void CreateQRCodeWorker(object code)
        {
            QRCodeOption codeOption = (QRCodeOption)code;
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hints.Add(EncodeHintType.MARGIN, 0);
            
            
            UnityMainThreadDispatcher.Instance.Enqueue(() => {

                BitMatrix bitMatrix = new MultiFormatWriter().encode(codeOption.url, BarcodeFormat.QR_CODE, codeOption.size.x, codeOption.size.y, hints);
                if (bitMatrix==null)
                {
                    Debug.LogError("Create QRCode ERROR");
                }
                Texture2D texture2D = new Texture2D(codeOption.size.x, codeOption.size.y);
                for (int x = 0; x < codeOption.size.y; x++)
                {
                    for (int y = 0; y < codeOption.size.x; y++)
                    {
                        if (bitMatrix[x, y])
                        {
                            texture2D.SetPixel(y, x, Color.black);
                        }
                        else
                        {
                            texture2D.SetPixel(y, x, Color.white);
                        }

                    }
                }
                texture2D.Apply();
                CreateQRCodeTexture2D = texture2D;
                CreateQRCodeEnded = true;
                CreateQRCodingFlag = false;
            });
        }

        /// <summary>
        /// 扫描QRCode
        /// </summary>
        /// <param name="color32s"></param>
        /// <param name="camWidth"></param>
        /// <param name="camHeight"></param>
        /// <returns></returns>
        public string ScanQRCode(Color32[] color32s,int camWidth,int camHeight)
        {

            BarcodeReader barcodeReader = new BarcodeReader();
            Result tResult = barcodeReader.Decode(color32s, camWidth, camHeight);
            if (tResult!=null)
            {
                return tResult.Text;
            }
            else
            {
                return null;
            }
        }


        private void Update()
        {
            //不在生成状态
            if (CreateQRCodingFlag==false)
            {
                //生成完毕
                if (CreateQRCodeEnded)
                {
                    //生成完毕之后归位
                    CreateQRCodeEnded = false;
                    CreateQRCodingFlag = false;
                    CreateQRCodeEndEvent?.Invoke(CreateQRCodeTexture2D);
                }
            }
            else
            {
                //正在生成
            }
        }
    }
}
