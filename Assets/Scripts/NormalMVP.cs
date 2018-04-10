using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMVP : MonoBehaviour {

    Renderer myRenderer;
	// Use this for initialization
	void Start () {
        myRenderer = gameObject.GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        //Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        V[0, 0] = 1;
        V[0, 1] = 2;
        V[0, 2] = 2;
        //V[0, 3] = 25;
        V[1, 0] = 2;
        V[1, 1] = 12;
        V[1, 2] = 2;
        //V[1, 3] = 2;
        V[2, 0] = 2;
        V[2, 1] = 2;
        V[2, 2] = -1;
        //V[2, 3] = 2;
        V[3, 0] = 0;
        V[3, 1] = 0;
        V[3, 2] = 0;
        //V[3, 3] = -2;

        //Matrix4x4 M = myRenderer.localToWorldMatrix;
        //Matrix4x4 MVP = P * V * M;
        Matrix4x4 projection = Matrix4x4.Ortho(-100.0f, 100.0f, -100.0f, 100.0f, 1.0f, 100.0f);
        myRenderer.material.SetMatrix("_MATRIX_V", V);
        myRenderer.material.SetVector("_CAM_POS", Camera.main.transform.position);
        myRenderer.material.SetMatrix("_MATRIX_P", projection);
        //Matrix4x4 p = Camera.main.projectionMatrix;
        //p[2, 2] = -p[2, 2];
        //p[3, 2] = -p[3, 2];

        //Matrix4x4 mvp = p * Camera.main.transform.worldToLocalMatrix * transform.localToWorldMatrix;
        //myRenderer.material.SetMatrix("_MATRIX_V", mvp);
    }
}
