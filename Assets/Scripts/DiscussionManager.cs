using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DiscussionManager : NetworkBehaviour
{

    List<GameObject> playerHeroes = new List<GameObject>();

    [SyncVar]
    int playerCount = 0;

    int playerIndex = 0;

    public List<string> DisplayTexts;
    int currentStringIndex = 0;

    public GameObject speechBubblePrefab;
    GameObject speechBubble;

	// Use this for initialization
	void Start () {
        speechBubble = Instantiate(speechBubblePrefab);
        //updateSpeechBubble();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.N))
        {
            if(currentStringIndex < DisplayTexts.Count -1)
            {
                currentStringIndex++;
                updateSpeechBubble();
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(currentStringIndex > 0)
            {
                currentStringIndex--;
                updateSpeechBubble();
            }
        }

        if(playerHeroes.Count < playerCount)
        {
            playerHeroes.Clear();
            var heroes = GameObject.FindGameObjectsWithTag("Player");
            foreach( var go in heroes)
            {
                playerHeroes.Add(go);
            }
        }
        if (playerHeroes.Count > 0 && playerIndex < playerHeroes.Count)
        {
            speechBubble.transform.position = playerHeroes[playerIndex].transform.position;
            speechBubble.transform.position += new Vector3(1, 3, -0.1f);
            speechBubble.transform.localScale = new Vector3(0.05f, 0.05f, 1);
        }
    }

    void updateSpeechBubble()
    {
        Debug.Log(DisplayTexts[currentStringIndex]);

        var bubbleImage = speechBubble.transform.GetChild(0);
        if(bubbleImage != null)
        {
            var textGameObject = bubbleImage.GetChild(0);
            if(textGameObject != null)
            {
                textGameObject.GetComponent<Text>().text = DisplayTexts[currentStringIndex];
            }
        }
        if(isServer)
        {
            playerCount = NetworkServer.connections.Count;
        }
        playerIndex = currentStringIndex % playerCount;
        
    }
}
