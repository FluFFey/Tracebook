using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{

    public static MusicManager instance;
    
    AudioSource audioSource;
    public AudioClip[] songs;
    public AudioClip[] fanfares;
    public AudioClip[] titleSongs;
    [Range(0, 1)]
    public float volume;
    public float soundVolume = 1.0f;  //0-1
    public float musicVolume = 1.0f;  //0-1
    public float masterVolume = 1.0f; //0-1
    private bool musicMuted = false;
    public enum songEnums
    {
        MAIN_THEME,
        FANFARE,
        TITLE_SCREEN
    };



    void Awake()
    {
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
        audioSource = GetComponent<AudioSource>();
        masterVolume = 1.0f;
    }

    public void playSong(songEnums songType)
    {
        switch (songType)
        {
            case songEnums.TITLE_SCREEN:
                audioSource.clip = titleSongs[Random.Range(0, titleSongs.Length)];
                
                break;
            case songEnums.MAIN_THEME:
                //audioSource.clip = songs[Random.Range(0, songs.Length)];
                audioSource.clip = songs[3];
                break;
            case songEnums.FANFARE:
                audioSource.clip = fanfares[Random.Range(0, fanfares.Length)];
                break;
        }
        audioSource.loop = true;
        
        //yield return new WaitForSeconds(0.5f);
        StartCoroutine(setVolume(volume));
        audioSource.PlayDelayed(0.1f);
    }

    public IEnumerator silenceMusic(float setOver = 1.5f)
    {
        
        float curVol = audioSource.volume;
        for (float i = 0; i < setOver; i += Time.deltaTime)
        {
            audioSource.volume = curVol - curVol * (i / setOver) * masterVolume;
            yield return null;
        }
        audioSource.volume = 0;
    }
    public IEnumerator setVolume(float newVolume, float setOver = 1.5f, bool fromZero = true)
    {
        if (!musicMuted) //cba to deal with this shit right now. TODO: future: fix so modifications to sound stay even when muted
        {
            float curVol = fromZero ? 0 : audioSource.volume;
            for (float i = 0; i < setOver; i += Time.deltaTime)
            {
                audioSource.volume = curVol + (newVolume - curVol) * (i / setOver) * masterVolume;
                yield return null;
            }
            audioSource.volume = newVolume * masterVolume;
        }
        
    }
    
    public void toggleMusic()
    {
        musicMuted = !musicMuted;
        musicVolume = (Mathf.Round(musicVolume) + 1) % 2; //simplest for now. fix later maybe
        audioSource.volume = musicVolume*volume;
    }
    public void toggleSound()
    {

        soundVolume = (Mathf.Round(soundVolume) + 1) % 2; //simplest for now. fix later maybe
    }

    public void toggleMasterVolume()
    {
        masterVolume = (Mathf.Round(masterVolume) + 1) % 2; //simplest for now. fix later maybe
    }

    public void playFanfare()
    {
        audioSource.clip = fanfares[Random.Range(0, fanfares.Length)];// (int)songEnums.MAIN_THEME];
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.Play();
    }

    void Update()
    {
       // audioSource.volume = volume;
    }
}
