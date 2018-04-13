using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkRotationSync : NetworkBehaviour {
	
	// Update is called once per frame
	void Update ()
    {
        if(hasAuthority)
        {
            Cmd_updateZRot(transform.eulerAngles.z);
        }
	}

    [Command]
    private void Cmd_updateZRot(float newZRot)
    {
        Rpc_setZRot(newZRot);
    }

    [ClientRpc]
    private void Rpc_setZRot(float newZRot)
    {
        Vector3 newRot = transform.eulerAngles;
        newRot.z = newZRot;
        transform.eulerAngles = newRot;
    }

}
