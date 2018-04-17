using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour {
    public GameObject target;
    private float minimapMaxZoom = 10;
    private float minimapMinZoom = 25;
    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if(target !=null)
        {
            Vector3 newPos = target.transform.position;
            newPos.z = -10;
            transform.position = newPos;
        }   
	}
    public IEnumerator changeZoom(float fillPercent, GameObject sliderObj)
    {
        //StopAllCoroutines(); //this shit does not fucking work
        float zoomTime = 0.5f;
        float startSize = GetComponent<Camera>().orthographicSize;
        
        float goalZoom = 10 + (fillPercent * 15);
        float totalChange = goalZoom - startSize;

        Vector3 sliderStartPos = sliderObj.transform.localPosition;
        float totalSliderChange = -fillPercent - sliderStartPos.y+1;
        for (float f=0; f <zoomTime; f+=Time.deltaTime )
        {
            float pd = f / zoomTime;
            float endSize = startSize + totalChange * pd;
            Vector3 newSliderPos = sliderStartPos + pd*Vector3.up*totalSliderChange;
            GetComponent<Camera>().orthographicSize = endSize;
            sliderObj.transform.localPosition = newSliderPos;
            yield return null;
        }
    }
}
