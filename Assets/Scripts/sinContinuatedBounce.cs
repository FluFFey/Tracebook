using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sinContinuatedBounce : MonoBehaviour {

    public float loopTime;
    public float bounceDist;
    private Vector3 startPos;
    // Use this for initialization
    void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = startPos;
        newPos.y += Mathf.Sin((Time.time * 90 * 1.0f / loopTime) * Mathf.Deg2Rad) * bounceDist;
        transform.position = newPos;
	}
}
