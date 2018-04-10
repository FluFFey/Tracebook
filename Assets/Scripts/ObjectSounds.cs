using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSounds : MonoBehaviour
{
    public AudioClip[] sounds;
    private Rigidbody2D r2d;
    public float moveSoundDelay;
    public float pitchRange;
    private SoundCaller sc;
    private Timer soundTimer;
    // Use this for initialization
    void Start ()
    {
        soundTimer = new Timer(moveSoundDelay);
        r2d = GetComponent<Rigidbody2D>();
        sc = GetComponent<SoundCaller>();
    }
	
	//Have no good idea on how to make this dynamic atm
	void Update ()
    { 
        if (soundTimer.hasEnded())
        {            
            if (r2d != null && (!Mathf.Approximately(r2d.velocity.x, 0) || !Mathf.Approximately(r2d.velocity.y, 0)))
            {
                if (sc.attemptSound(sounds[Random.Range(0, sounds.Length)], pitchRange))
                {
                    soundTimer.restart();
                }
            }
        }
		
	}
}
