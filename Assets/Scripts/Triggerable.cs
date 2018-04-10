using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerable : MonoBehaviour {

    public void startTrigger(GameObject triggeringObject)//GameObject triggeringObject)
    {
        gameObject.SendMessage("trigger",triggeringObject, SendMessageOptions.DontRequireReceiver);//, triggeringObject);
    }
}
