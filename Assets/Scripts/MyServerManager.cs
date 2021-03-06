﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyServerManager : NetworkBehaviour {

    public static MyServerManager instance;
    public GameObject playerCharacterPrefab;
    public GameObject discussingPlayerPrefab;
    public GameObject networkManager;
    Color[] playerColors;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            DestroyObject(gameObject);
        }
        DontDestroyOnLoad(this);
        playerColors = new Color[] //apparently warcraft 3 colors 
        {
            Color.red,
            Color.blue,
            new Color32(0x00,0xFF,0xFF,0xFF),
            new Color32(0x80,0x00,0x80,0xFF),
            new Color32(0xFF,0xFF,0x00,0xFF),
            new Color32(0x00,0xFF,0x80,0xFF),
            new Color32(0x00,0xFF,0x00,0xFF),
            new Color32(0x00,0xFF,0x00,0xFF)
        };
    }
    
    // Use this for initialization
    void Start ()
    {

	}

    public void changeScene(string sceneName)
    {
        networkManager.GetComponent<NetworkManager>().ServerChangeScene(sceneName);
        //Rpc_startGame();

        foreach (NetworkConnection connection in NetworkServer.connections)
        {
            StartCoroutine(waitForClientSceneChange(connection,connection.connectionId));
        }
    }

    IEnumerator waitForClientSceneChange(NetworkConnection connection, int playerNo)
    {
        Timer timeOutTimer = new Timer(5.0f);
        //ClientScene.Ready(connection);
        while (!connection.isReady && !timeOutTimer.hasEnded())
        {
            yield return null;
        }
        if (connection.isReady)
        {
            GameObject go;
            if (SceneManager.GetActiveScene().name == "Discussion")
            {
                go = Instantiate(discussingPlayerPrefab);
            }
            else
            {
                go = Instantiate(playerCharacterPrefab);
            }

            go.GetComponent<PropBlockNetworkColorSetter>().setColor(playerColors[playerNo]);
            //MeshRenderer sphereMeshRenderer = go.transform.GetChild(0).GetComponent<MeshRenderer>();
            //MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            //sphereMeshRenderer.GetPropertyBlock(propBlock);
            //propBlock.SetColor("_Color", playerColors[playerNo]);
            //sphereMeshRenderer.SetPropertyBlock(propBlock);
            Vector3 newPos = go.transform.position;
            if (SceneManager.GetActiveScene().name == "Discussion")
            {
                newPos = new Vector3(-37.6f, 4.2f, 0);
            }
                newPos.x += (playerNo/2) * 2;
            newPos.y += (playerNo % 2) * 2;
            go.transform.position = newPos;
            //Now that object is on server, propagate to all clients
            NetworkServer.SpawnWithClientAuthority(go, connection);
            
            Rpc_initiateScene(go);
        }
        else
        {
            print("Connection " + connection.address + " timed out");
        }
    }

    [ClientRpc]
    void Rpc_initiateScene(GameObject go)
    {

        if (go.GetComponent<InputHandler>() != null && 
            go.GetComponent<InputHandler>().hasAuthority)
        {
            Camera.main.GetComponent<CameraScript>().target = go;
            if (GameObject.Find("minimapCam") != null)
            {
                GameObject.Find("minimapCam").GetComponent<MinimapScript>().target = go;
            }
        }
    }

    public bool amIServer()
    {
        return isServer;
    }

    //[Command]
    //void Cmd_spawnHero(int i)
    //{
    //    //print(i);
    //    //Guaranteed to be on server right now
    //    GameObject go = Instantiate(playerCharacterPrefab);
    //    go.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", playerColors[4]);
    //    print(go.transform.GetChild(0).name);
    //    //Now that object is on server, propagate to all clients
    //    NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    //}

    public int getConnectionCount()
    {
        return NetworkServer.connections.Count;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
