using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManager : MonoBehaviour {

    public static TimeManager instance;
    public float gameTimeMultiplier = 1.0f;
    public float gameDeltaTime;
    public float fixedGameDeltaTime;
    public float fps;
    private Text fpsText;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            DestroyObject(gameObject);
        }
        
        
        
    }

    void Start()
    {
        if (GameObject.Find("HUD"))
        {
            fpsText = GameObject.Find("HUD").GetComponentInChildren<Text>();
            fpsText.gameObject.SetActive(false);
        }
    }

    public void pauseGameTime()
    {
        gameTimeMultiplier = 0.0f;
    }
    public void resumeGameTime()
    {
        gameTimeMultiplier = 1.0f;
    }
	
	// Update is called once per frame
	void Update () {
        fps = 1.0f / Time.deltaTime;
        if (fpsText)
        {
            fpsText.text = "FPS: " + fps;
            if (Input.GetKeyDown(KeyCode.F1))
            {
                fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
            }
        }
        
        
        gameDeltaTime = gameTimeMultiplier * Time.deltaTime;
    }
    void FixedUpdate()
    {
        fixedGameDeltaTime = gameTimeMultiplier * Time.fixedDeltaTime;
    }
}
