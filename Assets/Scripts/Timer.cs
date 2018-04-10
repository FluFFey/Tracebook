using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer {

    float duration;
    bool started;
    bool paused;
    private System.DateTime startTime;
    private System.DateTime timeOfPause;
    private System.TimeSpan pauseDuration;
    //duration in seconds
    public Timer(float duration, bool startOnInit = true) 
    {
        this.duration = duration;
        started = startOnInit;
        startTime = System.DateTime.Now;
        pauseDuration = System.TimeSpan.FromSeconds(0.0);
    }

    public void start()
    {
        startTime = System.DateTime.Now;
    }

    public void stop()
    {
        started = false;
    }

    public void restart()
    {
        paused = false;
        pauseDuration = System.TimeSpan.FromSeconds(0.0);
        started = true;
        startTime = System.DateTime.Now;
    }

    //PAUSE IS NOT TESTED!!! plz remember this when you need it
    //public void pause()
    //{
    //    if (started && !paused)
    //    {
    //        paused = true;
    //        timeOfPause = System.DateTime.Now;
    //    }

    //}

    //public void unpause()
    //{
    //    if (started && paused)
    //    {
    //        paused = false;
    //        pauseDuration = System.DateTime.Now.Subtract(timeOfPause);
    //    }
    //}

    public void setDuration(float duration)
    {
        this.duration = duration;
    }

    public float getElapsedTime()
    {
        if (paused)
        {
            return (float)startTime.Subtract(timeOfPause).TotalSeconds;
        }
        return (float)System.DateTime.Now.Subtract(startTime).Subtract(pauseDuration).TotalSeconds;
        
    }

    public bool isStarted()
    {
        return started;
    }
    public bool isPaused()
    {
        return paused;
    }

    public bool hasEnded()
    {
        return getElapsedTime() > duration;
    }

}
