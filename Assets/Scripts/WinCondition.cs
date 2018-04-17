using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

using UnityEngine;

public class WinCondition : NetworkBehaviour {

    BoxCollider2D collider;
    List<GameObject> players = new List<GameObject>();
	// Use this for initialization
	void Start () {
        collider = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {

        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                MyServerManager.instance.changeScene("Discussion");
            }

            var connectionCount = MyServerManager.instance.getConnectionCount();
            if (players.Count == MyServerManager.instance.getConnectionCount())
            {
                MyServerManager.instance.changeScene("Discussion");
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            players.Add(collision.gameObject);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            players.Remove(collision.gameObject);
        }
    }

}
