using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private static readonly string[] levelNames =
    {
        "level0",
        "tutorial1",
        "level1",
        "level2",
        "level3"
    };
    private float timeAtStartOfGame = -1;
    private static int levelIndex = 0;
    public static LevelManager instance;
    private static int currentLevel;
    public AudioClip GameCompleteFanfare;
    private SoundCaller sc;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            DestroyObject(gameObject);
        }
        DontDestroyOnLoad(this);

        sc = GetComponent<SoundCaller>();
    }

    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void triggerGameOver(string deathReason = "Hello darkness")
    {
        TimeManager.instance.pauseGameTime();
        PauseMenuScript.instance.children[0].SetActive(!PauseMenuScript.instance.children[0].activeSelf);
        PauseMenuScript.instance.children[0].transform.GetChild(1).GetComponent<Text>().text = deathReason;
    }
    
    public void loadMainMenu()
    {
        timeAtStartOfGame = -1;
        SceneManager.LoadScene("StartGame");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "StartGame")
        {
            bool pauseOpen = PauseMenuScript.instance.children[1].activeSelf;
            if (pauseOpen)
            {
                TimeManager.instance.resumeGameTime();
                PauseMenuScript.instance.children[1].SetActive(false);
                
            }
            else
            {
                TimeManager.instance.pauseGameTime();
                PauseMenuScript.instance.children[1].SetActive(true);
            }
        }
    }

    void Start()
    {
        //Old code. don't think it does anything anymore
        //currentLevel = 0;
        ////DEBUG: this whole check if 100% for debugging so you can start scene from different levels but still hear music and transition to next level
        ////assumes all level start with L/l and 6th element is levelNumber
        //if (SceneManager.GetActiveScene().name[0] != 'L' && SceneManager.GetActiveScene().name[0] != 'l')
        //{
        // //   StartCoroutine(nextLevel());
        //}
        //else
        //{

        //    currentLevel = int.Parse(SceneManager.GetActiveScene().name[5].ToString());
        //   // print(currentLevel);
        //    //MusicManager.instance.playSong();
        //}

    }

    public void initiateNextLevel()
    {
        //has to be like this because THIS object has to know when loading is complete, and coroutines have to go in order
        //TODO: probably don't HAVE to be.. but this is my solution for now
        if (instance == this) 
        {
            StartCoroutine(nextLevel());
        }
    }

    public void loadLevel(int levelToLoad)
    {
        if (levelToLoad ==0)
        {
            timeAtStartOfGame = Time.time;
        }
        levelIndex = levelToLoad;
        StartCoroutine(nextLevel());
    }

    public IEnumerator nextLevel()
    {
        if (levelIndex >= levelNames.Length)
        {
            TimeManager.instance.pauseGameTime();
            yield return new WaitForSeconds(1.5f);
            PauseMenuScript.instance.children[3].SetActive(true);
            sc.attemptSound(GameCompleteFanfare);
            if (timeAtStartOfGame != -1)
            {   
                GameObject.Find("Hero(Clone)").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                int min = (int)((Time.time - timeAtStartOfGame)/60.0f);
                int sec = (int)((Time.time - timeAtStartOfGame) % 60.0f);
                PauseMenuScript.instance.children[3].GetComponentInChildren<Text>().text = "Congratulations! You completed the demo in " + min + ":" + sec + "!";
            }
            else
            {
                PauseMenuScript.instance.children[3].GetComponentInChildren<Text>().text = "Congratulations! You completed the final level! Try from the beginning!";
            }
            
        }
        else
        {
            if (SceneManager.GetActiveScene().name != "StartGame")
            {
                yield return StartCoroutine(Camera.main.GetComponent<CameraScript>().fade(false, 4));
            }

            string nextLvlName = "Scenes/" + levelNames[levelIndex];
            levelIndex++;
            /*
            string nextLvlName = "Scenes/level" + currentLevel.ToString(); //important: all scenes have to be in Scenes folder
            currentLevel++;
            */
            //if (!SceneManager.GetSceneByName(nextLvlName).IsValid()) //TODO: can't get this to work for now. cba to fix
            {

                //print(nextLvlName + " not found. end of available levels reached"); //TOOD: add victory screen or whatever
                //  return;
            }
            //else
            {
                print(nextLvlName);
                AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(nextLvlName);
                int loadingFrames = 0;
                while (!asyncLoadLevel.isDone)
                {
                    //loading screen stuff. probably fine without loading screen x - D
                    loadingFrames++;
                    print("loading frames:" + loadingFrames);
                    yield return null;
                }
                MusicManager.instance.playSong(MusicManager.songEnums.MAIN_THEME);
            }
        }
        
    }
}
