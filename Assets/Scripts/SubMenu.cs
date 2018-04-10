using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SubMenu : MonoBehaviour {

    
    public enum BUTTONS
    {
        NEW_GAME,
        LEVEL_SELECT,
        EXIT,
        BACK,
        BACK_FROM_PLAYING_MENU, //TODO: remember to do something about this if this system is ever used again
        SELECT_LEVEL,
        RESUME_GAME,
        RESTART_LEVEL,
        SETTINGS,
        TOGGLE_MUSIC,
        TOGGLE_SOUNDS,
        TO_MAIN_MENU
    }

    public int defaultButton = 0;
    //public int menuOptionsWidth = 1; //1 means "no" width. as you only go down
    private GameObject[] menuObjects; //are identified by being texts. all need TextPulse script
    private int highlightedButton;
    private bool directionDown = false;
    private SoundCaller sc;
    public AudioClip[] changeMenuObject;
    // Use this for initialization
    public BUTTONS[] buttonIDs; //IMPORTANT! need to be as long as number of buttons
	void Start () {
        sc = gameObject.GetComponent<SoundCaller>();
        int i = 0;
        menuObjects = new GameObject[gameObject.GetComponentsInChildren<TextPulse>().Length];
		foreach (TextPulse menuObject in gameObject.GetComponentsInChildren<TextPulse>())
        {
            menuObjects[i] = menuObject.gameObject;
            menuObjects[i].GetComponent<TextPulse>().setButtonInfo(i, (int)buttonIDs[i]);
            i++;
        }
        highlightedButton = 0;
        menuObjects[highlightedButton].GetComponent<TextPulse>().highlight();
	}

    void OnEnable()
    {
        changeHighlightedButton(defaultButton);
    }
    public void changeHighlightedButton(int newButton)
    {
        if (newButton != highlightedButton)
        {
            menuObjects[highlightedButton].GetComponent<TextPulse>().highlight(false);
            highlightedButton = newButton;
            sc.attemptSound(changeMenuObject[Random.Range(0, changeMenuObject.Length - 1)], 0.02f);
            menuObjects[highlightedButton].GetComponent<TextPulse>().highlight();
        }
        
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetAxis("Vertical") != 0 && directionDown == false)
        {
            directionDown = true;            
            int newButton = (highlightedButton + Mathf.RoundToInt(Input.GetAxis("Vertical")*-1)/**menuOptionsWidth*/) % menuObjects.Length; //*-1 because we add buttons from top to buttom, while axis go from bottom to top
            if (newButton < 0)
            {
                newButton = menuObjects.Length -1;
            }
            changeHighlightedButton(newButton);
        }

        //if (Input.GetAxis("Horizontal") != 0 && directionDown == false)
        //{
        //    directionDown = true;
        //    int newButton = (highlightedButton + Mathf.RoundToInt(Input.GetAxis("Horizontal") * -1) * menuOptionsWidth) % menuObjects.Length; //*-1 because we add buttons from top to buttom, while axis go from bottom to top
        //    if (newButton < 0)
        //    {
        //        newButton = menuObjects.Length - 1;
        //    }
        //    changeHighlightedButton(newButton);
        //}

        if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
        {
            directionDown = false;
        }
    }
}
