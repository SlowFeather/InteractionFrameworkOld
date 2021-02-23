using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INIDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //设置保存文件路径
        IniStorage.mINIFileName = Application.streamingAssetsPath + "/IFConfig/config.ini";
        //写入一个bool型变量
        IniStorage.WriteBool("TestBool", IniStorage.SectionName.config, true);
        //读取这个变量
        Debug.Log(IniStorage.GetBool("TestBool",IniStorage.SectionName.config,false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
