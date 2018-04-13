using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    public static PauseMenuScript instance;
    public GameObject[] children;
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
        children = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            children[i] =transform.GetChild(i).gameObject;
            children[i].SetActive(false);
        }

    }
}
