using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBlockNetworkColorSetter : MonoBehaviour {

    private Color sphereColor;

    // Use this for initialization
    void Start () {
        MeshRenderer sphereMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        sphereMeshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", sphereColor);
        sphereMeshRenderer.SetPropertyBlock(propBlock);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void setColor(Color color)
    {
        sphereColor = color;
    }

    internal Color getColor()
    {
        return sphereColor;
    }
}
