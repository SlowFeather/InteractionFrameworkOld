using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleQRCode : MonoBehaviour {
    public string code;
    public Vector2Int codeSize;

    public RawImage rawImage;

    private QRCodeTool codeTool;

    private void Start()
    {
        codeTool = new QRCodeTool();
    }

    public void NormalCreate() {
        rawImage.texture = codeTool.NormalEncodeQRCode(code, codeSize);
    }

    public void AsyncCreate() {
        if (codeTool.isEncodeing() == false) {
            codeTool.EncodeQRCode(code, codeSize);
            StartCoroutine(getQRCode());
        }
    }

    IEnumerator getQRCode() {
        while (true) {
            if (codeTool.GetQRimage() != null) {
                rawImage.texture = codeTool.GetQRimage();
                break;
            }
            yield return 0;
        }
    }
}
