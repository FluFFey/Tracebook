using UnityEngine;
using System.Collections;
using System.Linq;
public class SoundCaller : MonoBehaviour
{

    AudioSource[] fxSources;
    public float volume = 1.0f;
    void Awake()
    {
        fxSources = GetComponents<AudioSource>();
    }

    //free up saved source. ONLY PASS RESERVED SOURCES
    public void freeSource(AudioSource sourceToFree)
    {
        bool success = true;
        foreach (var source in GetComponents<AudioSource>())
        {
            if (sourceToFree == source && success)
            {
                foreach (var currentSource in fxSources)
                {
                    if (sourceToFree == currentSource) //source must be source from this component, but not one already in the array
                    {
                        print("error freeing audio source. tried to free source which wasn't reserved");
                        success = false;
                        break;
                    }
                }
            }
        }
        if (success)
        {
            //this MIGHT be error-prone I think, But wanted to test with array instead of list for fun
            //creates a new array which contains old array + new source
            fxSources = fxSources.Concat(Enumerable.Repeat(sourceToFree, 1)).ToArray();
        }
    }

    //used when you want a source to be saved for a specific sound
    public AudioSource reserveSource()
    {
        if (fxSources.Length > 0)
        {
            AudioSource returnSource = fxSources[fxSources.Length - 1];
            fxSources = fxSources.Take(fxSources.Length - 1).ToArray(); //basically remakes whole array. dunno if there is a faster way
            return returnSource;
        }

        print("no audio sources to reserve. Improve handling if crash. remove print message if it doesn't crash?");
        return default(AudioSource);
    }

    public AudioSource findFreeAudioSource()
    {
        foreach (var fxSource in fxSources)
        {
            if (!fxSource.isPlaying)
            {
                return fxSource;
            }
        }
        //no free audiosource
        return default(AudioSource);
    }

    //returns true if sound was played
    public bool attemptSound(AudioClip clip, float pitchRange = 0.0f)
    {
        AudioSource source = findFreeAudioSource();
        if (source != default(AudioSource)) //found available source
        {
            source.volume = volume*MusicManager.instance.soundVolume;
            source.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
            source.clip = clip;
            source.Play();
            return true;
        }
        return false;
    }
}
