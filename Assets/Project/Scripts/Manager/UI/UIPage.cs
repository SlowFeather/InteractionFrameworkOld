using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPage : MonoBehaviour
{
    public void Initialize()
    {
        OnInitialize();
    }
    public void Open(object arg)
    {
        
        gameObject.SetActive(true);
        OnOpen(arg);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        OnClose();
    }
    protected virtual void OnInitialize() { }
    protected virtual void OnOpen(object arg) { }
    protected virtual void OnClose() { }
}
