using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TextPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text txt;
    private bool highlighted = false;
    private Vector3 startScale;
    private int buttonID;
    private int buttonType;
    
    //keep comments in case they fall out. (don't think they can, but not sure)
    //public Color inactiveColor = new Color(0 / 255.0f, 150 / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f); //this is beautiful and you know it
    //public Color highlightedColor = new Color(0 / 255.0f, 0 / 255.0f, 255.0f / 255.0f, 255.0f);
    public Color inactiveColor;
    public Color highlightedColor;
    bool mouseOver = false;
    //SubMenu parentSubMenu;
    void Awake()
    {
      //  parentSubMenu = transform.parent.gameObject.GetComponent<SubMenu>();
        txt = GetComponent<Text>();
        txt.color = inactiveColor;
        startScale = transform.localScale;
    }
    public void setButtonInfo(int id,int type)
    {
        buttonID= id;
        buttonType = type;
    }

    public void highlight(bool toHighlightButton = true)
    {
        highlighted = toHighlightButton;
        if (!highlighted)
        {
            txt.color = inactiveColor;
            transform.localScale = startScale;
            //mouseOver = false;
        }
        if (highlighted)
        {
            txt.color = highlightedColor;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        transform.parent.GetComponent<SubMenu>().changeHighlightedButton(buttonID);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        transform.parent.GetComponent<SubMenu>().changeHighlightedButton(buttonID);
    }
    // Update is called once per frame
    void Update()
    {
        //float pulseSpeed = 5.0f;
        if (highlighted)
        {
            //transform.localScale = Vector3.one + (Vector3.one + Vector3.one * Mathf.Sin(Time.time * pulseSpeed)) * 0.04f;
            if (Input.GetKeyUp(KeyCode.Space) ||
            Input.GetKeyUp(KeyCode.Return) ||
            (Input.GetMouseButtonUp(0) && mouseOver))
            {
                trigger();
            }
        }
        
        if (mouseOver && !highlighted && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            mouseOver = false;
            transform.parent.GetComponent<SubMenu>().changeHighlightedButton(buttonID);
        }
    }

    public void trigger()
    {
        
        switch(buttonType)
        {
            case (int)SubMenu.BUTTONS.NEW_GAME:
                StartCoroutine(startNewGame());
                break;
            case (int)SubMenu.BUTTONS.EXIT:
                Application.Quit();
                break;
            case (int)SubMenu.BUTTONS.SELECT_LEVEL:
                int level = (int)char.GetNumericValue(name[6]); //this it not just hacky-ish, this is hacky as fuck. CBA to add custom system for level select..
                LevelManager.instance.loadLevel(level);
                break;
            case (int)SubMenu.BUTTONS.BACK:
                transform.parent.parent.GetChild(0).gameObject.SetActive(true);
                transform.parent.gameObject.SetActive(false);
                break;
            case (int)SubMenu.BUTTONS.BACK_FROM_PLAYING_MENU: //TODO: fix when rework
                transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                transform.parent.gameObject.SetActive(false);
                transform.parent.gameObject.GetComponent<SubMenu>().buttonIDs[2] = SubMenu.BUTTONS.BACK;
                break;
            case (int)SubMenu.BUTTONS.RESUME_GAME:
                TimeManager.instance.resumeGameTime();
                transform.parent.gameObject.SetActive(false);
                break;
            case (int)SubMenu.BUTTONS.RESTART_LEVEL:
                TimeManager.instance.resumeGameTime();
                LevelManager.instance.restartLevel();
                transform.parent.gameObject.SetActive(false);
                break;
            case (int)SubMenu.BUTTONS.TO_MAIN_MENU:
                TimeManager.instance.resumeGameTime();
                LevelManager.instance.loadMainMenu();
                MusicManager.instance.playSong(MusicManager.songEnums.TITLE_SCREEN);
                transform.parent.gameObject.SetActive(false);
                break;
            case (int)SubMenu.BUTTONS.TOGGLE_MUSIC:
                MusicManager.instance.toggleMusic();
                break;
            case (int)SubMenu.BUTTONS.TOGGLE_SOUNDS:
                MusicManager.instance.toggleSound();
                break;
            default:
                break;

        }
    }
    IEnumerator startNewGame()
    {
        foreach (Transform childTransform in transform.parent.GetComponentInChildren<Transform>())
        {
            if (childTransform.GetComponent<SpriteRenderer>()!=null)
            {
                childTransform.GetComponent<SpriteRenderer>().enabled = false;
                childTransform.GetComponent<TextPulse>().enabled = false;
            }
        }

        //StartCoroutine(Tools.moveObject(heroOnMenu, Vector3.left, 1.6f, 80.0f));
        //heroOnMenu.GetComponent<Image>().sprite = heroLookingRightSprite;
        StartCoroutine(Camera.main.GetComponent<CameraScript>().fade(false, 1.75f));
        
        yield return MusicManager.instance.silenceMusic(1.75f);
        yield return new WaitForSeconds(0.8f);
        //LevelManager.instance.loadLevel(0);
        MyServerManager.instance.changeScene("level0");
    }
}
