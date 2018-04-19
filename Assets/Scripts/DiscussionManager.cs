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
	void Start ()
    {
        DisplayTexts.Add("Hey, good job finding a room!");
        DisplayTexts.Add("Yeah, location sharing on Tracebook is super smart!");
        DisplayTexts.Add("Oh yes, it is a clever usage of location sharing");
        DisplayTexts.Add("It is super useful!But we must remember to not share it with anyone.");
        DisplayTexts.Add("Huh, that does not matter because I share my location on Facebook all the time. Connecting to friends and family is really valuable you know.");
        //DisplayTexts.Add("Connecting to friends and family is really valuable you know.");
        DisplayTexts.Add("Yes, but information about our address and location should not be shared to strangers ??");
        DisplayTexts.Add("But I am only sharing it on Facebook, not publicly?");
        DisplayTexts.Add("You must remember that good and bad news travels fast online. What we are sharing could be virtually available forever!!");
        //DisplayTexts.Add("What we are sharing could be virtually available forever!!");
        DisplayTexts.Add("Huh… That is true.But who is interested in us anyways?");
        DisplayTexts.Add("I don’t know. But it is important to be Internet Smart.");
        DisplayTexts.Add("Yes.I don’t want to get in a tricky situation in the future.");
        DisplayTexts.Add("I know, something could have lasting consequences. Maybe in 10 years!");
        DisplayTexts.Add("It is important to have some privacy.");
        DisplayTexts.Add("That is why I like Tracebook. It is so effective, and I am glad that only we can see it!");
        DisplayTexts.Add("Wait, did you not read the terms and conditions?");
        DisplayTexts.Add("No, why?");
        DisplayTexts.Add("By using this app, tracebook owns the rights to the data gathered");
        DisplayTexts.Add("...");
        DisplayTexts.Add("They can sell it to anyone they want!");
        DisplayTexts.Add("?!");
        DisplayTexts.Add("This can be used to create a virtual profile of us");
        DisplayTexts.Add("Just like Facebook did with the online data harvesting??");
        DisplayTexts.Add("Yes! This could be used to target marketing at us!");
        DisplayTexts.Add("So they sell our information to other people?");
        DisplayTexts.Add("Yeah, and they use us only for the money!");
        DisplayTexts.Add("But..that’s not too scary?");
        DisplayTexts.Add("Well, if the security is breached, anyone can see where we are!");
        DisplayTexts.Add("Anyone?!");
        DisplayTexts.Add("Yes, even bullies and stalkers, even your MOM!!");
        DisplayTexts.Add("gee, i hope that doesn’t happen!");
        DisplayTexts.Add("It i important to know the terms of access applications have.");
        DisplayTexts.Add("Yeah, I have heard that some could be really over-privileged..");
        DisplayTexts.Add("What does that mean?");
        DisplayTexts.Add("Not all applications need permission to access location... But they can still ask for it, and sell the information to others.");
        DisplayTexts.Add("But they can still ask for it, and sell the information to others.");
        DisplayTexts.Add("Wow, I will be more carefully with that now..");
        DisplayTexts.Add("You should be.");
        DisplayTexts.Add("But why should we use Tracebook then, and other applications?");
        DisplayTexts.Add("Because..I wouldn’t have found the room that fast if it was not for Tracebook!");

        speechBubble = Instantiate(speechBubblePrefab);
        updateSpeechBubble();
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
