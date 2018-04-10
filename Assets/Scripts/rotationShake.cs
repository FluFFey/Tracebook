using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationShake : MonoBehaviour {

    Vector3 startRotation;
	// Use this for initialization
	void Start ()
    {
        startRotation = transform.rotation.eulerAngles;
        startRotation.x -= 5;
        startRotation.y -= 5;
        StartCoroutine(shakeLamp());
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    IEnumerator shakeLamp()
    {
        yield return 0;
    //    StartCoroutine(shakeLamp(endOfXRot, ))
    }
}
