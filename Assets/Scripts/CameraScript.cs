using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public GameObject target; //target to follow
    public Vector2 targetOffset;
    public Color fadeColor;
    public float orthographicPlaySize = 3.5f;
    private Texture2D fadeTexture;
    public float defaultFadeTime;
    private float startDistance;
    private int cameraCinematicDistance = 3;
    public float followSpeed = 0.33f;
    private void Awake()
    {
        startDistance = transform.position.z;
    }
    void Start () {
        targetOffset = new Vector3(0, 0, 0);
        fadeTexture = new Texture2D(1, 1,TextureFormat.ARGB32,false);
        fadeTexture.SetPixel(0, 0, fadeColor);
        fadeTexture.Apply();
        StartCoroutine(fade(true, defaultFadeTime));
    }
    public float getStartDistance()
    {
        
        return startDistance;
    }

    public IEnumerator levelTransitionZoom(GameObject elevator, float duration, float newZ)
    {
        Vector3 camOrigin = transform.position;
        camOrigin.z = -1;// target != null ? target.transform.position.z : ;
        target = null;
        Vector3 endPos = elevator.transform.position;
        endPos.y -= cameraCinematicDistance;
        endPos.z -= 1;
        Vector3 lookatPos = camOrigin;
        Vector3 directionVector = (endPos- transform.position).normalized;
        float distance = (endPos - transform.position).magnitude; //TODO: maybe add overload for moveobject which takes just one vector
        StartCoroutine(Tools.moveObject(gameObject, directionVector, duration, distance));

        float startSize = GetComponent<Camera>().orthographicSize;
        float goalSize = 1.0f;
        
        for (float i = 0; i < duration; i += TimeManager.instance.gameDeltaTime)
        {

            float pd = i / duration;
            lookatPos = camOrigin + (elevator.transform.position - camOrigin) * pd;
            transform.LookAt(lookatPos);
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(startSize, goalSize, pd);
            yield return null;
        }
    }
    public IEnumerator levelTransitionZoom2(GameObject elevator, float duration, float newZ) //TODO: "needs" to be rewritten
    {
        Vector3 camOrigin;
        camOrigin.z = -1;
        Vector3 endPos = elevator.transform.position;
        endPos.y -= cameraCinematicDistance;
        endPos.z = newZ;
        camOrigin = transform.position;
        Vector3 endView = camOrigin;
        endView.y -= cameraCinematicDistance;
        endView.z = -1;
        
        //camOrigin.z = -1;
        Vector3 lookatPos = camOrigin;
        Vector3 directionVector = (endPos - transform.position).normalized;
        float distance = (endPos - transform.position).magnitude; //TODO: maybe add overload for moveobject which takes just one vector
        StartCoroutine(Tools.moveObject(gameObject, directionVector, duration, distance,Tools.INTERPOLATION_TYPE.SMOOTH,3));

        float startSize = GetComponent<Camera>().orthographicSize; 
        float goalSize = orthographicPlaySize;

        for (float i = 0; i < duration; i += TimeManager.instance.gameDeltaTime)
        {
            float pd = i / duration;
            lookatPos = elevator.transform.position + (Vector3.down*3/*endView - camOrigin*/) * pd;
            transform.LookAt(lookatPos);
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(startSize, goalSize, pd*pd*pd);
            yield return null;
        }
    }
    public IEnumerator gameOverZoom(float animationDuration)
    {
        targetOffset = Vector2.zero;
        Vector3 newPos = transform.position;
        float pd;
        float zoomDuration = animationDuration * 0.66f;

        //orthographic zoom
        float startSize = Camera.main.orthographicSize;
        float endSize = 2.0f;
        for (float i = 0; i < zoomDuration; i += TimeManager.instance.gameDeltaTime)
        {
            pd = i / zoomDuration;
            newPos = transform.position; //needed because position moves between updates because of offset
            Camera.main.orthographicSize = Mathf.Lerp(startSize, endSize, i * i * i);
            yield return null;
        }

        //perspective zoom
        float startZDistance = transform.position.z;
        float zDistancePostZoom = -6.0f;
        for (float i = 0; i < zoomDuration; i += TimeManager.instance.gameDeltaTime)
        {
            pd = i / zoomDuration;
            newPos = transform.position; //needed because position moves between updates because of offset
            newPos.z = startZDistance * (1 - pd) + zDistancePostZoom * (pd);
            transform.position = newPos;
            yield return null;
        }
    }
    // Update is called once per frame
    void FixedUpdate ()
    {
        if (target != null)
        {
            Vector3 distanceVector = target.transform.position + (new Vector3(targetOffset.x, targetOffset.y, 0)) - transform.position;
            float distanceFromTarget = distanceVector.magnitude;
            transform.Translate(new Vector3(distanceVector.x, distanceVector.y, 0) * distanceFromTarget * followSpeed * TimeManager.instance.fixedGameDeltaTime);
        }
        
	}

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.width), fadeTexture);
    }

    public IEnumerator fade(bool fadeIn, float fadeTime = 1.0f)
    {
        float multiplier = fadeIn ? -1.0f : 1.0f;
        float start = fadeIn ? 1.0f : 0.0f;
        float a = 0;
        for (float i = 0; i < fadeTime; i+= TimeManager.instance.gameDeltaTime)
        {
            a = (i / fadeTime) * multiplier;
            fadeTexture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, start+a));
            fadeTexture.Apply();
            yield return null;
        }
        fadeTexture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, start + multiplier));
        fadeTexture.Apply();
    }
}
