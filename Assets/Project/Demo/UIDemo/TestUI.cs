using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : UIPage
{
    protected override void OnInitialize()
    {
        base.OnInitialize();
        Debug.Log("Test UI OnInitialize");
    }

    protected override void OnOpen(object arg)
    {
        base.OnOpen(arg);
        Debug.Log("Test UI OnOpen");

    }

    protected override void OnClose()
    {
        base.OnClose();
        Debug.Log("Test UI OnClose");

    }

    public void Hello()
    {
        Debug.Log("Hello");
    }
}
