using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


public class SecondTestNode : Node {

    [Input] public float input2;

    public object GetValue()
    {
        return GetInputValue<object>("input2");
    }

}