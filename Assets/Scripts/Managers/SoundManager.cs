using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;
    private SoundCaller sc;

    public AudioClip newMessageSound;
    public AudioClip sharedLocationSound;

    public enum SOUNDS
    {
        NEW_MESSAGE,
        SHARED_LOCATION
    }

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            DestroyObject(gameObject);
            //Destroy(this);
        }
        DontDestroyOnLoad(this);
        sc = GetComponent<SoundCaller>();
    }

    public void playSound(SOUNDS sound)
    {
        AudioClip clipToPlay = null;
        switch (sound)
        {
            case SOUNDS.NEW_MESSAGE:
                clipToPlay = newMessageSound;
                break;
            case SOUNDS.SHARED_LOCATION:
                clipToPlay = sharedLocationSound;
                break;
            default:
                print("invalid sound requested");
                break;
        }

        sc.attemptSound(clipToPlay,0.03f,1.0f);
    }

    public SoundCaller getSoundCaller()
    {
        return sc;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
