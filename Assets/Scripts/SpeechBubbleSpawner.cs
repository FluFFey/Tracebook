using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleSpawner : MonoBehaviour
{

    public GameObject speechBubblePrefab;
    private GameObject speechBubble;
    private float timeSinceTextSpawn = 0.0f;
    private float textDuration = 3.0f;

    //int counter = 0;

    bool visible = false;
    // Use this for initialization
    void Start()
    {
        speechBubble = Instantiate(speechBubblePrefab, transform.position + new Vector3(0.5f, 0.7f, 0.0f), Quaternion.identity) as GameObject;
        speechBubble.transform.parent = transform;
        speechBubble.SetActive(visible);
    }
    // Update is called once per frame
    void Update()
    {

        timeSinceTextSpawn += TimeManager.instance.gameDeltaTime;

        //hack for spawning text bubble
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (true)//!visible)
            {
                //SpawnSpeechBubble("hey text this is pretty long to please fit the image");
            }
            else
            {
                //DestroySpeechBubble();
            }
        }

        FadeSpeechBubble();
    }

    private void FadeSpeechBubble()
    {
        float fadeOutStart = textDuration / 2;
        if (fadeOutStart < timeSinceTextSpawn)
        {
            var bubbleImage = speechBubble.transform.GetChild(0).GetComponent<Image>();
            if (bubbleImage != null)
            {
                var imageColor = bubbleImage.color;
                bubbleImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, 2 * (textDuration - timeSinceTextSpawn) / textDuration);
            }
            else
            {
                Debug.Log("failed to find image");
            }
        }
        if (timeSinceTextSpawn > textDuration)
        {
            DestroySpeechBubble();
        }
    }

    public void DestroySpeechBubble()
    {
        visible = false;
        speechBubble.SetActive(false);
    }

    public void SpawnSpeechBubble(string displayText, float duration = 5.0f)
    {
        DestroySpeechBubble();
        textDuration = duration;
        var bubbleImage = speechBubble.transform.GetChild(0);
        if (bubbleImage != null)
        {
            var textGameObject = bubbleImage.GetChild(0);
            if (textGameObject != null)
            {
                timeSinceTextSpawn = 0.0f;
                visible = true;
                speechBubble.SetActive(true);
                bubbleImage.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                textGameObject.GetComponent<Text>().text = displayText;
            }
            else
            {
                Debug.Log("Textarea not found");
            }
        }
        else
        {
            Debug.Log("Bubble image not found!");
        }
    }
}
