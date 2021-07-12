using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

// public classes deriving from Node are registered as nodes for use within a graph
public class MathNode : Node
{
    // Adding [Input] or [Output] is all you need to do to register a field as a valid port on your node 
    // 要将字段注册为节点上的有效端口，只需添加[Input]或[Output]
    [Input] public float a;
    [Input] public float b;
    // The value of an output node field is not used for anything, but could be used for caching output results
    // 输出节点字段的值不用于任何内容，但可以用于缓存输出结果
    [Output] public float result;
    [Output] public float sum;

    // The value of 'mathType' will be displayed on the node in an editable format, similar to the inspector
    // “mathType”的值将以可编辑的格式显示在节点上，类似于inspector
    public MathType mathType = MathType.Add;
    public enum MathType { Add, Subtract, Multiply, Divide }

    // GetValue should be overridden to return a value for any specified output port
    // 应重写GetValue以返回任何指定输出端口的值
    public override object GetValue(NodePort port)
    {
        // Get new a and b values from input connections. Fallback to field values if input is not connected
        // 从输入连接获取新的a和b值。如果输入未连接，则返回字段值
        float a = GetInputValue<float>("a", this.a);
        float b = GetInputValue<float>("b", this.b);

        // After you've gotten your input values, you can perform your calculations and return a value
        // 得到输入值后，可以执行计算并返回值
        if (port.fieldName == "result")
            switch (mathType)
            {
                case MathType.Add: default: return a + b;
                case MathType.Subtract: return a - b;
                case MathType.Multiply: return a * b;
                case MathType.Divide: return a / b;
            }
        else if (port.fieldName == "sum") return a + b;
        else return 0f;
    }
}