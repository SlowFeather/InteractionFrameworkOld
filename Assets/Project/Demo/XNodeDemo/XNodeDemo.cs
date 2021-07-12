using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XNodeDemo : MonoBehaviour
{
    public XNodeTest xNodeTest;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("当前节点："+xNodeTest.nodes[0].name);
        Debug.Log("下一个节点：" + xNodeTest.nodes[0].GetOutputPort("num").Connection.node.name);


        Debug.Log("当前节点：" + xNodeTest.GetFirstNode().name);
        Debug.Log("最后节点：" + xNodeTest.GetEndNode().name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
