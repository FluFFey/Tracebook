using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

using UnityEngine;

public class WinCondition : NetworkBehaviour {

    BoxCollider2D b2collider;
    List<GameObject> players = new List<GameObject>();
    bool sceneChangeInitiated = false;
    Color oldCol;
	// Use this for initialization
	void Start () {
        b2collider = GetComponent<BoxCollider2D>();
        oldCol = GetComponent<SpriteRenderer>().color;
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
            if (players.Count == MyServerManager.instance.getConnectionCount() &&
                 !sceneChangeInitiated)
            {
                MyServerManager.instance.changeScene("Discussion");
                sceneChangeInitiated = true;
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<InputHandler>().hasAuthority)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        
        if(collision.gameObject.CompareTag("Player"))
        {
            players.Add(collision.gameObject);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<InputHandler>().hasAuthority)
        {
            GetComponent<SpriteRenderer>().color = oldCol;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            players.Remove(collision.gameObject);
        }
    }

}
