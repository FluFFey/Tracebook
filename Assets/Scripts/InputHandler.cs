//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        GOOD_JOB,
        RETURN,
        NO_OF_MESSAGE_TYPES
    }

    public enum EMOJIS
    {
        ROFLMAO,
        POO,
        MONKEY,
        RETURN,
        NO_OF_EMOJIS
    }

    public enum LOCATION_BUTTONS
    {
        SHARE_LOCATION,
        ZOOM_IN,
        ZOOM_OUT,
        RETURN,
        NO_OF_BUTTONS
    }


    public enum PHONE_OBJECTS
    {
        MESSAGE,
        SHARE_LOCATION,
        EMOJI,
        NO_OF_OBJECTS //always at the end. don't set custom values to the ones above
    }
    int currentSelectedObject;

    public enum PHONE_MENUS
    {
        MAIN,
        MESSAGES,
        LOCATION,
        EMOJI,
        NO_OF_MENUS //always at the end. don't set custom values to the ones above
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
    private Vector2 lastDirection;
    public GameObject messagesPanel; //UI-panel where text messages appear
    public GameObject messagePrefab; //Prefab for message popup
    public Sprite flippedBubbleImage;
    private GameObject phone;
    PHONE_MENUS currentMenu;
    GameObject currentMenuObj;
    private float mapZoom = 1; //Don't think too much about this one
    private GameObject zoomSliderImg;
    private void updatePhoneHighlight()
    {
        GameObject menu = null;

        int noOfMessages = 0;

        switch (currentMenu) //when updating this, remember to have buttons first in the hierarchy
        {
            case PHONE_MENUS.MAIN:
                noOfMessages = (int)PHONE_OBJECTS.NO_OF_OBJECTS;
                menu = phone.transform.GetChild(0).gameObject;
                break;
            case PHONE_MENUS.MESSAGES:
                noOfMessages = (int)TEXT_MESSAGES.NO_OF_MESSAGE_TYPES;
                menu = phone.transform.GetChild(1).gameObject;
                break;
            case PHONE_MENUS.LOCATION:
                noOfMessages = (int)LOCATION_BUTTONS.NO_OF_BUTTONS;
                menu = phone.transform.GetChild(2).gameObject;
                break;
            case PHONE_MENUS.EMOJI:
                noOfMessages = (int)EMOJIS.NO_OF_EMOJIS;
                menu = phone.transform.GetChild(3).gameObject;
                break;
            //when updating this, remember to have buttons first in the hierarchy
            default:
                print("invalid menu");
                break;
        }

        MaterialPropertyBlock[] propBlocks = new MaterialPropertyBlock[noOfMessages];
        for (int i = 0; i < noOfMessages; i++)
        {
            propBlocks[i] = new MaterialPropertyBlock();
            menu.transform.GetChild(i).GetComponent<SpriteRenderer>().GetPropertyBlock(propBlocks[i]);

            propBlocks[i].SetFloat("_Outline", 0);
            if (i == currentSelectedObject)
            {
                propBlocks[i].SetFloat("_Outline", 1);
            }
            menu.transform.GetChild(i).GetComponent<SpriteRenderer>().SetPropertyBlock(propBlocks[i]);
        }
    }

    private void Awake()
    {
        
        if (messageDictionary == null)
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
        currentSelectedObject = 0;
        phone = GameObject.Find("PhoneCanvas").transform.GetChild(0).gameObject;
        zoomSliderImg = phone.transform.GetChild(2).GetChild(5).GetChild(2).gameObject; //This is fine
        //phoneCanvas.SetActive(false);
        currentMenuObj = phone.transform.GetChild(0).gameObject;
        //phone.SetActive(false);

        messagesPanel = GameObject.Find("MessagePanel");
        if (hasAuthority)
        {
            transform.GetChild(0).gameObject.layer = 10; //For showing color on minimap
        }
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
                transform.eulerAngles = new Vector3(0, 0, 307.0f);
                break;
            case (int)MOVE_DIRECTION.DOWN:
                transform.eulerAngles = new Vector3(0, 0, 127.0f);
                break;
            case (int)MOVE_DIRECTION.LEFT:
                transform.eulerAngles = new Vector3(0, 0, 37.0f);
                break;
            case (int)MOVE_DIRECTION.RIGHT:
                transform.eulerAngles = new Vector3(0, 0, 217.0f);
                break;
            case (int)MOVE_DIRECTION.UPLEFT:
                transform.eulerAngles = new Vector3(0, 0, 352.0f);
                break;
            case (int)MOVE_DIRECTION.UPRIGHT:
                transform.eulerAngles = new Vector3(0, 0, 262.0f);
                break;
            case (int)MOVE_DIRECTION.DOWNLEFT:
                transform.eulerAngles = new Vector3(0, 0, 82.0f);
                break;
            case (int)MOVE_DIRECTION.DOWNRIGHT:
                transform.eulerAngles = new Vector3(0, 0, 172.0f);
                break;
            default:
                break;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (currentSelectedObject != -1)
            {
                findPhoneMenuAction();
            }
            
        }
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    findPhoneMenuAction();

        //}
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    if (phone.activeSelf)
        //    {
        //        phone.SetActive(false);
        //    }
        //    else
        //    {
        //        phone.SetActive(true);
        //        switchPhoneMenu(PHONE_MENUS.MAIN);
        //    }
        //}

        if (phone.activeSelf)
        {
            int menuNo = (int)currentMenu;
            int count = 0;
            currentSelectedObject = -1;
            foreach (MouseOverObj childTransform in phone.transform.GetChild(menuNo).GetComponentsInChildren<MouseOverObj>())
            {
                if (phone.transform.GetChild(menuNo).GetChild(count).GetComponent<MouseOverObj>()!=null &&
                    phone.transform.GetChild(menuNo).GetChild(count).GetComponent<MouseOverObj>().isMouseOver)
                {
                    currentSelectedObject = count;
                    phone.transform.GetChild(menuNo).GetChild(count).GetComponent<MouseOverObj>().isMouseOver = false;
                }
                count++;
            }
            updatePhoneHighlight();


            //int numberToModWith = 0;
            //switch (currentMenu)
            //{
            //    case PHONE_MENUS.MAIN:
            //        numberToModWith = (int)PHONE_OBJECTS.NO_OF_OBJECTS;
            //        break;
            //    case PHONE_MENUS.MESSAGES:
            //        numberToModWith = (int)TEXT_MESSAGES.NO_OF_MESSAGE_TYPES;
            //        break;
            //    case PHONE_MENUS.EMOJI:
            //        numberToModWith = (int)EMOJIS.NO_OF_EMOJIS;
            //        break;
            //}

            //if (Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    switch (currentMenu)
            //    {
            //        case PHONE_MENUS.MESSAGES:
            //            if ((TEXT_MESSAGES)currentSelectedObject == TEXT_MESSAGES.RETURN)
            //            {
            //                currentSelectedObject = (int)TEXT_MESSAGES.RETURN-1;
            //            }
            //            else if (currentSelectedObject == 0 || currentSelectedObject == 1)
            //            {
            //                currentSelectedObject = (int)TEXT_MESSAGES.RETURN;
            //            }
            //            else
            //            {
            //                currentSelectedObject = mod((int)currentSelectedObject - 2, numberToModWith); //sweet
            //            }
            //            break;
            //        case PHONE_MENUS.EMOJI:
            //            if ((EMOJIS)currentSelectedObject == EMOJIS.RETURN)
            //            {
            //                currentSelectedObject = (int)EMOJIS.RETURN - 1;
            //            }
            //            else if (currentSelectedObject == 0 || currentSelectedObject == 1)
            //            {
            //                currentSelectedObject = (int)EMOJIS.RETURN;
            //            }
            //            else
            //            {
            //                currentSelectedObject = mod((int)currentSelectedObject - 2, numberToModWith); //sweet
            //            }
            //            break;
            //        default:
            //            //isok. PHONE_MENUS.MAIN will go here
            //            currentSelectedObject = mod((int)currentSelectedObject + 2, numberToModWith); //sweet
            //            break;
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    switch (currentMenu)
            //    {
            //        case PHONE_MENUS.MESSAGES:
            //            if ((TEXT_MESSAGES)currentSelectedObject == TEXT_MESSAGES.RETURN)
            //            {
            //                currentSelectedObject = 0;
            //            }
            //            else if ((TEXT_MESSAGES)currentSelectedObject == TEXT_MESSAGES.GOOD_JOB)
            //            {
            //                currentSelectedObject = (int)TEXT_MESSAGES.RETURN;
            //            }
            //            else
            //            {
            //                currentSelectedObject = mod((int)currentSelectedObject + 2, numberToModWith); //sweet
            //            }
            //            break;
            //        case PHONE_MENUS.EMOJI:
            //            if ((EMOJIS)currentSelectedObject == EMOJIS.RETURN)
            //            {
            //                currentSelectedObject = 0;
            //            }
            //            else if ((EMOJIS)currentSelectedObject == EMOJIS.MONKEY)
            //            {
            //                currentSelectedObject = (int)EMOJIS.RETURN;
            //            }
            //            else
            //            {
            //                currentSelectedObject = mod((int)currentSelectedObject + 2, numberToModWith); //sweet
            //            }
            //            break;
            //        default:
            //            //isok. PHONE_MENUS.MAIN will go here
            //            currentSelectedObject = mod((int)currentSelectedObject + 2, numberToModWith); //sweet
            //            break;
            //    }


            //}
            //if (Input.GetKeyDown(KeyCode.LeftArrow))
            //{
            //    currentSelectedObject = mod((int)currentSelectedObject - 1, numberToModWith); //sweet
            //}
            //if (Input.GetKeyDown(KeyCode.RightArrow))
            //{
            //    currentSelectedObject = (((int)currentSelectedObject + 1) % numberToModWith); //sweet
            //}
            updatePhoneHighlight();
        }

    }

    private void findPhoneMenuAction()
    {
        PHONE_MENUS menuAtTimeOfAction = currentMenu;
        switch(menuAtTimeOfAction)
        {
            case PHONE_MENUS.MAIN:
                switch (currentSelectedObject)
                {

                    case (int)PHONE_OBJECTS.MESSAGE:
                        switchPhoneMenu(PHONE_MENUS.MESSAGES);
                        break;
                    case (int)PHONE_OBJECTS.SHARE_LOCATION:
                        switchPhoneMenu(PHONE_MENUS.LOCATION);
                        break;
                    case (int)PHONE_OBJECTS.EMOJI:
                        switchPhoneMenu(PHONE_MENUS.EMOJI);
                        break;
                    default:
                        break;
                }
                break;
            case PHONE_MENUS.MESSAGES:
                SoundManager.instance.playSound(SoundManager.SOUNDS.NEW_MESSAGE);
                if ((TEXT_MESSAGES)currentSelectedObject == TEXT_MESSAGES.RETURN)
                {
                    switchPhoneMenu(PHONE_MENUS.MAIN);
                }
                else
                {
                    Cmd_sendTextMessage((TEXT_MESSAGES)currentSelectedObject);
                }                
                break;
            case PHONE_MENUS.EMOJI: //TODO: implement 
                if ((EMOJIS)currentSelectedObject == EMOJIS.RETURN)
                {
                    switchPhoneMenu(PHONE_MENUS.MAIN);
                }
                else
                {
                    //displayemoji((EMOJIS)currentSelectedObject); //TODO: fix
                }
                break;
            case PHONE_MENUS.LOCATION:
                switch (currentSelectedObject)
                {
                    case (int)LOCATION_BUTTONS.SHARE_LOCATION:
                        Cmd_createLocationSharer();
                        break;
                    case (int)LOCATION_BUTTONS.ZOOM_IN:
                        mapZoom += 0.33f;
                        mapZoom = mapZoom > 1.0f ? 1.0f : mapZoom;
                        StartCoroutine(GameObject.Find("minimapCam").GetComponent<MinimapScript>().changeZoom(mapZoom, zoomSliderImg));
                        break;
                    case (int)LOCATION_BUTTONS.ZOOM_OUT:
                        mapZoom -= 0.33f;
                        mapZoom = mapZoom < 0.0f ? 0.0f : mapZoom;
                        StartCoroutine(GameObject.Find("minimapCam").GetComponent<MinimapScript>().changeZoom(mapZoom, zoomSliderImg));
                        break;
                    case (int)LOCATION_BUTTONS.RETURN:
                        switchPhoneMenu(PHONE_MENUS.MAIN);
                        break;
                    default:
                        print("Choky");
                        break;
                }
                break;
            default:
                break;
        }
    }

    private void switchPhoneMenu(PHONE_MENUS newMenu)
    {
        if (currentMenuObj !=null)
        {
            currentMenuObj.SetActive(false);
        }
        currentMenu = newMenu;
        currentSelectedObject = 0;
        switch (newMenu)
        {
            case PHONE_MENUS.MAIN:
                phone.transform.GetChild(0).gameObject.SetActive(true);
                currentMenuObj = phone.transform.GetChild(0).gameObject;                
                break;
            case PHONE_MENUS.MESSAGES:
                phone.transform.GetChild(1).gameObject.SetActive(true);
                currentMenuObj = phone.transform.GetChild(1).gameObject;
                break;
            case PHONE_MENUS.EMOJI:
                phone.transform.GetChild(3).gameObject.SetActive(true);
                currentMenuObj = phone.transform.GetChild(3).gameObject;
                break;
            case PHONE_MENUS.LOCATION:
                phone.transform.GetChild(2).gameObject.SetActive(true);
                currentMenuObj = phone.transform.GetChild(2).gameObject;
                break;
            default:
                print("invalid phonemenu");
                break;
        }
    }

    //TRUE modulo. works properly for negative numbers
    int mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }

    [Command]
    void Cmd_createLocationSharer()
    {
        GameObject go = Instantiate(sharedLocationGO, transform.position, Quaternion.identity);
        //MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        //transform.GetChild(0).GetComponent<MeshRenderer>().GetPropertyBlock(propBlock);
        //Color newCol = propBlock.GetColor("_Color");
        Color sphereColor = GetComponent<PropBlockNetworkColorSetter>().getColor();
        go.GetComponent<SpriteRenderer>().color = sphereColor;
        NetworkServer.Spawn(go);
        Rpc_setLocationSharerColor(go, sphereColor);
        float locationShareLifetime = 10.0f;
        Destroy(go, locationShareLifetime);
    }

    [ClientRpc]
    void Rpc_setLocationSharerColor(GameObject locationShareGO, Color newColor)
    {
        SoundManager.instance.playSound(SoundManager.SOUNDS.SHARED_LOCATION);
        locationShareGO.GetComponent<SpriteRenderer>().color = newColor;
    }

    [Command]
    void Cmd_sendTextMessage(TEXT_MESSAGES messageType)
    {
        Rpc_spawnChatBubble(messageType);
    }

    [ClientRpc]
    void Rpc_spawnChatBubble(TEXT_MESSAGES messageType)
    {
        for (int i = 0; i < messagesPanel.transform.childCount; i++)
        {
            StartCoroutine(moveBubble(messagesPanel.transform.GetChild(i).gameObject));
        }
        //SoundManager.instance.playSound(SoundManager.SOUNDS.NEW_MESSAGE);
        GameObject messageGo = Instantiate(messagePrefab, messagesPanel.transform);

        messageGo.transform.GetChild(0).GetComponent<Text>().text = messageDictionary[messageType];
        messageGo.GetComponent<Image>().color = GetComponent<PropBlockNetworkColorSetter>().getColor();

        if (!hasAuthority)
        {
            messageGo.GetComponent<RectTransform>().pivot = new Vector2(0,0.5f);
            messageGo.GetComponent<RectTransform>().anchorMax = messageGo.GetComponent<RectTransform>().pivot;
            messageGo.GetComponent<RectTransform>().anchorMin = messageGo.GetComponent<RectTransform>().pivot;   
            messageGo.GetComponent<Image>().sprite = flippedBubbleImage;
            StartCoroutine(scaleBubble(messageGo));
        }
        Vector3 newPos = new Vector3(0, -190);
        messageGo.GetComponent<RectTransform>().anchoredPosition = newPos;
        StartCoroutine(scaleBubble(messageGo));
        StartCoroutine(destroyBubbleOverTime(messageGo));
    }


    IEnumerator destroyBubbleOverTime(GameObject bubble)
    {
        yield return new WaitForSeconds(4.0f);
        float fadeTime = 2.0f;
        Color newBubbleCol = bubble.GetComponent<Image>().color;
        Color newTextCol = bubble.transform.GetChild(0).GetComponent<Text>().color;
        for (float f=0; f< fadeTime; f +=Time.deltaTime)
        {
            float pd = f / fadeTime;
            //Color newColor = startBubbleCol;
            newBubbleCol.a = 1 - pd;
            newTextCol.a = 1 - pd;
            bubble.GetComponent<Image>().color = newBubbleCol;
            bubble.transform.GetChild(0).GetComponent<Text>().color = newTextCol;
            yield return null;
        }
        Destroy(bubble);
    }

    IEnumerator scaleBubble(GameObject bubble)
    {
        float scaleTime = 0.33f;
        for (float f = 0; f < scaleTime; f += Time.deltaTime)
        {
            float pd = f / scaleTime;
            Vector3 newScale = Vector3.one*pd;
            bubble.transform.localScale = newScale;
            yield return null;
        }
        bubble.transform.localScale = Vector3.one;
    }

    IEnumerator moveBubble(GameObject bubble)
    {
        float moveTime = 0.33f;
        Vector3 startPos = bubble.transform.position;
        for (float f = 0; f < moveTime; f+=Time.deltaTime)
        {
            float pd = f / moveTime;
            Vector3 newPos = startPos + (Vector3.up*120.0f*pd);
            if (bubble != null) //might be destroyed during movement from timeout
            {
                bubble.transform.position = newPos;
            }
            yield return null;
        }
    }



    private void updateCamera()
    {
        Camera.main.GetComponent<CameraScript>().targetOffset = newVelocity.normalized*0.0f;
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

        //if (phone.activeSelf) //For keyboard input
        //{
        //    moveDirection = 0;
        //    newVelocity = Vector2.zero;
        //    handleVelocity();
        //    return;
        //}
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