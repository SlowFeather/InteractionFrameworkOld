using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INIDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IniStorage.WriteBool("TestBool", IniStorage.SectionName.config, true);

        Debug.Log(IniStorage.GetBool("TestBool",IniStorage.SectionName.config,false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
