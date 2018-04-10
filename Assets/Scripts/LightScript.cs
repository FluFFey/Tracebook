using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class LightScript : MonoBehaviour {
    public bool startOn; //more readable
    private bool triggered;
    Light lightComponent;
    // Use this for initialization
    
    void Awake()
    {
        lightComponent = GetComponent<Light>();
      //  Shader.SetGlobalFloat("_AreaLightRange", lightComponent.range);
    }

    void Start()
    {
        triggered = startOn;
        lightComponent.enabled = startOn;
    }
    public void trigger()
    {
        triggered = !triggered;
        lightComponent.enabled = triggered;
    }

    
    void Update()
    {
     //   Shader.SetGlobalFloat("_AreaLightRange", lightComponent.range);
    }
}
