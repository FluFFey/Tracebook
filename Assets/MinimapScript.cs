using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour {
    public GameObject target;
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 newPos = target.transform.position;
        newPos.z = -10;
        transform.position = newPos;
	}
}
