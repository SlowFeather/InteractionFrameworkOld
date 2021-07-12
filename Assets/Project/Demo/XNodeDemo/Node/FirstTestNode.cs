using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class FirstTestNode : Node {

	[Output] public float num;

	[ShowAssetPreview]
	public Sprite sprite;
	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return num;
	}
}