using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PropBlockNetworkColorSetter : NetworkBehaviour{

    [SyncVar]
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
        //MeshRenderer sphereMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        //MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        //sphereMeshRenderer.GetPropertyBlock(propBlock);
        //propBlock.SetColor("_Color", sphereColor);
        //sphereMeshRenderer.SetPropertyBlock(propBlock);
        //if (!isServer)
        //{

        //    print("not server" + sphereMeshRenderer.material.GetColor("_Color"));
        //}
    }

    internal void setColor(Color color)
    {
        sphereColor = color;
        //MeshRenderer sphereMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        //MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        //sphereMeshRenderer.GetPropertyBlock(propBlock);
        //propBlock.SetColor("_Color", sphereColor);
        //sphereMeshRenderer.SetPropertyBlock(propBlock);
        //print(sphereMeshRenderer.material.GetColor("_Color"));
    }

    internal Color getColor()
    {
        return sphereColor;
    }
}
