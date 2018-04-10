using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerOnCollision : MonoBehaviour {

    public List<GameObject> triggerList;

    // Use this for initialization
    void Start () {
        //triggerList = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (GameObject obj in triggerList)
            {
                if (obj != null)
                {
                    obj.GetComponent<Triggerable>().startTrigger(collision.gameObject);
                }
            }
        }
    }

}
