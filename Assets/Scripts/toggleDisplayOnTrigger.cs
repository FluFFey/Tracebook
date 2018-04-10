using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleDisplayOnTrigger : MonoBehaviour {

    public bool startVisible;
    public bool canBeDisabled;
    public bool canBeEnabled;
    private bool isVisible;
    [Range(1.0f,5.0f)]
    public float fadeTime;
	// Use this for initialization
	void Start () {
        isVisible = startVisible ? true : false;
        if (!isVisible)
        {
            Color newColor = GetComponent<SpriteRenderer>().color;
            newColor.a = 0;
            GetComponent<SpriteRenderer>().color = newColor;
        }
	}
	
    public void trigger()
    {
        if ((!isVisible && canBeEnabled)
            || isVisible && canBeDisabled)
        {
            StopCoroutine(changeVisibility()); //doesn't seem to do jack shit
            StartCoroutine(changeVisibility());
        }
    }

    IEnumerator changeVisibility()
    {
        float start = isVisible ? 1 : 0;
        float goal = isVisible ? 0 : 1;
        Color newColor = GetComponent<SpriteRenderer>().color;
        for (float i = 0; i < fadeTime; i+= TimeManager.instance.gameDeltaTime)
        {
            newColor.a = (goal * (i / fadeTime)) + start*(1-(i/fadeTime));// curAlpha;
            GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }
        isVisible = !isVisible;
    }

}
