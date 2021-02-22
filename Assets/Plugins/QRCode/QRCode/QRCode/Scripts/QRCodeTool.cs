using UnityEngine;
using System.Threading;
using ZXing;
using ZXing.Common;
using System.Collections.Generic;

public class QRCodeTool {

    private bool _isEncodeing = false;

    private int imgWidth = 0;

    private int imgHeight = 0;

    private Texture2D texture2D;

    private BitMatrix bitMatrix;

    public bool isEncodeing() {
        return _isEncodeing;
    }

    /// <summary>
    /// 获取通过异步生成的二维码Texture2D信息
    /// </summary>
    /// <returns></returns>
    public Texture2D GetQRimage() {
        if (!_isEncodeing) {
           texture2D = new Texture2D(imgWidth, imgHeight);
            for (int x = 0; x < imgHeight; x++)
            {
                for (int y = 0; y < imgWidth; y++)
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

            return texture2D;
        }
        return null;
    }

    /// <summary>
    /// 通过单独线程生成二维码(异步)
    /// </summary>
    /// <param name="code"></param>
    /// <param name="vector2"></param>
    public void EncodeQRCode(string code , Vector2 vector2) {
        if (!_isEncodeing && code != null) {
            texture2D = null;
            _isEncodeing = true;
            imgWidth = (int)vector2.x;
            imgHeight = (int)vector2.y;
            Thread task = new Thread(new ParameterizedThreadStart(encode));
            task.Start(code);
        }
    }

    /// <summary>
    /// 通过主线程生成二维码，返回二维码的Texture2D
    /// </summary>
    /// <param name="code"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Texture2D NormalEncodeQRCode(string code, Vector2Int size)
    {

        Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();

        hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");

        BitMatrix _bitMatrix = new MultiFormatWriter().encode(code, BarcodeFormat.QR_CODE, size.x, size.y, hints);

        if (_bitMatrix == null) return null;

        texture2D = new Texture2D(size.x, size.y);
        for (int x = 0; x < size.y; x++)
        {
            for (int y = 0; y < size.x; y++)
            {
                if (_bitMatrix[x, y])
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
        return texture2D;
    }

    private void encode(object code) {

        Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();

        hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");

        bitMatrix = new MultiFormatWriter().encode(code as string, BarcodeFormat.QR_CODE, imgWidth, imgHeight, hints);

        _isEncodeing = false;
    }
}
