﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyServerManager : NetworkBehaviour {

    public static MyServerManager instance;
    public GameObject playerCharacterPrefab;
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
        playerColors = new Color[]
        {
            Color.red,
            Color.blue,
            new Color32(0x00,0xFF,0xFF,0x00),
            new Color32(0x80,0x00,0x80,0x00),
            new Color32(0xFF,0xFF,0x00,0x00),
            new Color32(0x00,0xFF,0x80,0x00),
            new Color32(0x00,0xFF,0x00,0x00),
            new Color32(0x00,0xFF,0x00,0xFF)
        };
    }
    
    // Use this for initialization
    void Start ()
    {

	}
	
    void OnPlayerConnected(NetworkPlayer player)
    {
        
        //print(player.);
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
            GameObject go = Instantiate(playerCharacterPrefab);
            

            go.GetComponent<InputHandler>().setColor(playerColors[playerNo]);
            //MeshRenderer sphereMeshRenderer = go.transform.GetChild(0).GetComponent<MeshRenderer>();
            //MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            //sphereMeshRenderer.GetPropertyBlock(propBlock);
            //propBlock.SetColor("_Color", playerColors[playerNo]);
            //sphereMeshRenderer.SetPropertyBlock(propBlock);

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
        if (go.GetComponent<InputHandler>().hasAuthority)
        {
            Camera.main.GetComponent<CameraScript>().target = go;
            GameObject.Find("minimapCam").GetComponent<MinimapScript>().target = go;
        }
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

    // Update is called once per frame
    void Update ()
    {
		
	}
}
