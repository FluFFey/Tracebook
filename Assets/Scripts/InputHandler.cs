//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InputHandler : NetworkBehaviour
{
    public enum HERO_STATE
    {
        IDLE,
        MOVING,
        DISABLED,
        NO_OF_STATES
    }

    public enum TEXT_MESSAGES
    {
        GATHER_HERE,
        HELP,
        RUN,
        GOOD_JOB
    }

    private static Dictionary<TEXT_MESSAGES,string> messageDictionary;


    enum MOVE_DIRECTION
    {
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
        UPLEFT = UP + LEFT,
        UPRIGHT = UP + RIGHT,
        DOWNLEFT = DOWN + LEFT,
        DOWNRIGHT = DOWN + RIGHT
    }
    int moveDirection;
    public int moveSpeed = 1;
    public GameObject sharedLocationGO;

    private Vector2 newVelocity;
    public HERO_STATE state;
    private float fixedDt;
    
    private SoundCaller sc;
    private Rigidbody2D rb2d;
    [SyncVar]
    private Color sphereColor;
    private Vector2 lastDirection;

    private void Awake()
    {
        if (messageDictionary != null)
        {
            messageDictionary = new Dictionary<TEXT_MESSAGES, string>();
            messageDictionary[TEXT_MESSAGES.GATHER_HERE] = "I found something";
            messageDictionary[TEXT_MESSAGES.HELP] = "Help me!";
            messageDictionary[TEXT_MESSAGES.RUN] = "Run";
            messageDictionary[TEXT_MESSAGES.GOOD_JOB] = "Well done";
        }
        moveDirection = (int)MOVE_DIRECTION.LEFT;
        rb2d = GetComponent<Rigidbody2D>();
        sc = GetComponent<SoundCaller>();
        lastDirection = Vector2.right;
        
    }

    private void Start()
    {
        MeshRenderer sphereMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        sphereMeshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", sphereColor);
        sphereMeshRenderer.SetPropertyBlock(propBlock);
    }

    bool isCollidingWithFloor(Vector3 point)
    {
        RaycastHit hit;
        var collisionFound = Physics.Raycast(new Vector3(point.x, point.y, 1), new Vector3(0,0,-1), out hit);
        if (collisionFound)
        {
            //Hit is detected, you can now check with the RaycastHit how far away and what object you hit and apply logic there.
            Debug.Log("Raycast hit object " + hit.transform.name);
            Debug.Log("Distance between object and " + hit.transform.name + ": " + hit.distance);

            return true;
        }
        else
        {
            Debug.Log("raycast found nothing");
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        //still runs on all inputhandlers, not just ones you own
        //How to verify if I'm authorised to change object?
        if(!hasAuthority)
        {
            return;
        }

        switch (state)
        {
            case HERO_STATE.DISABLED:
                break;
            case HERO_STATE.IDLE:
                break;
            case HERO_STATE.MOVING:
                updateCamera();
                break;
            default:
                break;
        }
        switch(moveDirection)
        {
            case (int)MOVE_DIRECTION.UP:
                transform.eulerAngles = new Vector3(0, 0, -50.0f);
                break;
            case (int)MOVE_DIRECTION.DOWN:
                transform.eulerAngles = new Vector3(0, 0, 130.0f);
                break;
            case (int)MOVE_DIRECTION.LEFT:
                transform.eulerAngles = new Vector3(0, 0, 40.0f);
                break;
            case (int)MOVE_DIRECTION.RIGHT:
                transform.eulerAngles = new Vector3(0, 0, 200.0f);
                break;
            case (int)MOVE_DIRECTION.UPLEFT:
                transform.eulerAngles = new Vector3(0, 0, 355.0f);
                break;
            case (int)MOVE_DIRECTION.UPRIGHT:
                transform.eulerAngles = new Vector3(0, 0, 265.0f);
                break;
            case (int)MOVE_DIRECTION.DOWNLEFT:
                transform.eulerAngles = new Vector3(0, 0, 85.0f);
                break;
            case (int)MOVE_DIRECTION.DOWNRIGHT:
                transform.eulerAngles = new Vector3(0, 0, 175.0f);
                break;
            default:
                break;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Cmd_createLocationSharer();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Cmd_sendTextMessage(TEXT_MESSAGES.HELP);
        }
    }

    [Command]
    void Cmd_createLocationSharer()
    {
        GameObject go = Instantiate(sharedLocationGO, transform.position, Quaternion.identity);
        NetworkServer.Spawn(go);
        float locationShareLifetime = 10.0f;
        Destroy(go, locationShareLifetime);
    }


    [Command]
    void Cmd_sendTextMessage(TEXT_MESSAGES messageType)
    {

    }

    internal void setColor(Color color)
    {
        sphereColor = color;
    }

    private void updateCamera()
    {
        Camera.main.GetComponent<CameraScript>().targetOffset = newVelocity.normalized*1.2f;
    }

    public void changeHeroState(HERO_STATE newState)
    {
        state = newState;
        switch (newState)
        {
            case HERO_STATE.MOVING:
                break;
            case HERO_STATE.DISABLED:
                rb2d.velocity = Vector2.zero;
                break;
            case HERO_STATE.IDLE:
                break;
            default:
                break;
        }
    }


    void FixedUpdate ()
    {
        if(!hasAuthority)
        {
            return;
        }
        fixedDt = TimeManager.instance.fixedGameDeltaTime;

        newVelocity = Vector2.zero;
        float speed = 4 * TimeManager.instance.gameTimeMultiplier;
        newVelocity.y = Input.GetAxis("Vertical");
        newVelocity.x = Input.GetAxis("Horizontal");

        moveDirection = 0;
        if (newVelocity.y <0)
        {
            moveDirection += (int)MOVE_DIRECTION.DOWN;
        }
        else if (newVelocity.y > 0)
        {
            moveDirection += (int)MOVE_DIRECTION.UP;
        }
        if (newVelocity.x < 0)
        {
            moveDirection += (int)MOVE_DIRECTION.LEFT;
        }
        else if (newVelocity.x > 0)
        {
            moveDirection += (int)MOVE_DIRECTION.RIGHT;
        }
        changeHeroState((newVelocity.x == 0 && newVelocity.y == 0) ?
            HERO_STATE.IDLE :
            HERO_STATE.MOVING); //hurray for good code
        if (!(Mathf.Approximately(newVelocity.x,0) && Mathf.Approximately(newVelocity.y, 0)))
        {
            lastDirection = newVelocity;
        }           
        newVelocity = newVelocity.normalized * speed;
        handleVelocity();
    }
   

    private void handleVelocity() //should only be called from fixedUpdate
    {
        rb2d.velocity = newVelocity;
        //float distanceFromBreakingPoint = (transform.position - new Vector3(breakingPoint.x, breakingPoint.y, 0)).magnitude;
        //if (inLegalZoneNextFrame)
        //{
        //    breakingPoint = Vector2.zero;
        //    isRubberbanding = false;
        //    rubberBandParticlesChild.SetActive(false);
        //}
        //else if (breakingPoint == Vector2.zero)
        //{
        //    breakingPoint = transform.position;
        //    isRubberbanding = true;
        //    sc.attemptSound(barrierBreakSounds[Random.Range(0, barrierBreakSounds.Length)]);

        //}
        //else if (distanceFromBreakingPoint > distanceAllowedOutside)
        //{
        //    StartCoroutine(dashToBreakingPoint(distanceFromBreakingPoint));
        //}
        //else
        //{
        //    ParticleSystem.MainModule psm= rubberBandParticlesChild.GetComponent<ParticleSystem>().main;
        //    psm.startSpeed = distanceFromBreakingPoint;// *0.4f;
        //    rb2d.velocity *= Mathf.Pow((((distanceAllowedOutside* 1.5f) - distanceFromBreakingPoint) / (distanceAllowedOutside * 1.5f)),3);
        //    Vector2 distanceNormalized = new Vector2(transform.position.x - breakingPoint.x, transform.position.y - breakingPoint.y).normalized;
        //    Vector3 newRot = rubberBandParticlesChild.transform.rotation.eulerAngles;
        //    if (transform.position.y - breakingPoint.y > 0)
        //    {
        //        newRot.z = 90.0f + Mathf.Acos(distanceNormalized.x) * Mathf.Rad2Deg;
        //    }
        //    if (transform.position.y - breakingPoint.y <= 0)
        //    {
        //        newRot.z = 90.0f - Mathf.Acos(distanceNormalized.x) * Mathf.Rad2Deg;
        //    }
        //    rubberBandParticlesChild.transform.rotation = Quaternion.Euler(newRot);
        //    rubberBandParticlesChild.SetActive(true);

        //    switch (ghostState)
        //    {
        //        case GHOST_STATE.GHOST: //TODO: refactor
        //            GetComponent<SpeechBubbleSpawner>().SpawnSpeechBubble("The light, it burns!", 3);
        //            break;
        //        case GHOST_STATE.HUMAN:
        //            GetComponent<SpeechBubbleSpawner>().SpawnSpeechBubble("Darkness is scary!", 3);
        //            break;
        //        default:
        //            break;
        //    }


            
        //}
    }
    IEnumerator deathAnimation()
    {
        float deathTime = 3.0f;
        StartCoroutine(Camera.main.GetComponent<CameraScript>().gameOverZoom(deathTime));
        Vector3 newRot = transform.rotation.eulerAngles;
        Vector3 startScale = transform.localScale;
        Vector3 newScale = startScale;
        float pd;
        for (float i = 0; i < deathTime; i+=TimeManager.instance.gameDeltaTime)
        {
            pd = i / deathTime;
            newScale = new Vector2(startScale.x * (1.0f - pd) , startScale.y * (1.0f - pd));
            //newScale.x = startScale.x * (0.85f - pd)+0.15f;
            //newScale.x = startScale.y * (0.85f - pd) + 0.15f;
            newRot.z = i*720.0f;
            transform.rotation = Quaternion.Euler(newRot);
            transform.localScale = newScale;
            yield return null;
        }

        yield return null;
    }
}