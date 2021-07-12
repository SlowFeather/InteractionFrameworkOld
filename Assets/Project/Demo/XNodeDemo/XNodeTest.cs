using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName ="XNodeMenu",menuName = "XNodeTest")]
public class XNodeTest : NodeGraph { 
	public Node GetFirstNode()
    {
        return this.nodes[0];
    }

    public Node GetEndNode()
    {
        return this.nodes[nodes.Count-1];
    }
}