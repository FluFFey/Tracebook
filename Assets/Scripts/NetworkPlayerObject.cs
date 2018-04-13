using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

enum REMOTE_CALLS
{
    START_GAME
}

public class NetworkPlayerObject : NetworkBehaviour {

    public GameObject playerUnit;
    //public GameObject myPlayerUnit;
	// Use this for initialization
	void Start () {
        if (!hasAuthority)
        {
            return;
        }
        //since playerObject is invisible, create a physical object

        //Instantiate() only creates an object locally.
        //Still won't exist on server or other client, 
        //unless networkserver.spawn() is called on this object
       // Cmd_spawnHero();
	}
	
	// Update is called once per frame
	void Update () {
	       //runs on every computer, whether they own it(have authority) or not	
	}

    //[ClientRpc]
    //void Rpc_executeRemoteCall(REMOTE_CALLS call)
    //{
    //    switch(call)
    //    {
    //        case REMOTE_CALLS.START_GAME:
    //            break;
    //        default:
    //            print("Invalid remote call");
    //            break;
    //    }
    //}

    //Commands are special functions that ONLY get executed on the server
    //[Command]
    //void Cmd_spawnHero()
    //{
    //    //Guaranteed to be on server right now
    //    GameObject go = Instantiate(playerUnit);
    //    go = myPlayerUnit;
    //    //Now that object is on server, propagate to all clients
    //    NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    //}

}
