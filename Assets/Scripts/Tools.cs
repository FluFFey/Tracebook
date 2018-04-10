using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public enum INTERPOLATION_TYPE
    {
        LERP,
        SMOOTH
    }

    public static float lerp(float pd) //pd = percentage done
    {
        return pd;
    }

    public static float smoothInterpolation(float pd) //pd = percentage done
    {
        return pd * pd * (3 - 2 * pd);
    }

    private static System.Func<float, float> getInterpolationFunc(INTERPOLATION_TYPE type)
    {
        switch (type)
        {
            case INTERPOLATION_TYPE.LERP:
                return lerp;
            case INTERPOLATION_TYPE.SMOOTH:
                return smoothInterpolation;
            default:
                return smoothInterpolation;
        }
    }

    public static IEnumerator moveObject(GameObject go, Vector3 direction, float duration, float distance, INTERPOLATION_TYPE type = INTERPOLATION_TYPE.SMOOTH, int power = 1) //direction normalized
    {
        Vector3 startPos = go.transform.localPosition;
        System.Func<float, float> interpolationFunc = getInterpolationFunc(type);
        //float delta = 0.0f;
        //float oldPos = 0.0f;
        float newPos = 0.0f;

        for (float i = 0; i < duration; i += TimeManager.instance.gameDeltaTime)
        {
            //float pd = 0.25f + (i * 0.75f / duration); //TODO: make this line better mebbe
            float pd = i / duration; //TODO: ???
            newPos = interpolationFunc(pd);
            newPos = Mathf.Pow(newPos, power);
            //delta = newPos - oldPos;
            //oldPos = newPos;
            go.transform.localPosition=startPos+(direction * newPos * distance);
            yield return null;
        }
        go.transform.localPosition = startPos + (direction * distance);
    }

    //duration = duration for shake in 1 direction
    public static IEnumerator shakeObject(GameObject go, Vector3 axis, float degrees, float duration, int loops = 1)
    {
        float sumOfLoopsDuration = duration * loops;
        for (float j = 0; j < sumOfLoopsDuration; j += TimeManager.instance.gameDeltaTime)
        {
            float phase = Mathf.Sin((j / duration) * 2 * Mathf.PI);
            go.transform.rotation = Quaternion.Euler(phase * axis * degrees);
            yield return null;
        }    
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));//make sure we are straight!
    }

    public static IEnumerator rotateObject(GameObject go, Vector3 eulerAngles, float duration, INTERPOLATION_TYPE type = INTERPOLATION_TYPE.SMOOTH, int power = 1) //works!
    {
        Vector3 startAngles = go.transform.eulerAngles;
        System.Func<float, float> interpolationFunc = getInterpolationFunc(type);
        float newValue = 0.0f;
        float pd = 0;

        for (float i = 0; i < duration; i += TimeManager.instance.gameDeltaTime) //might be buggy. not tested super much
        {
            pd = i / duration;
            newValue = interpolationFunc(pd);
            newValue = Mathf.Pow(newValue, power);
            go.transform.rotation = Quaternion.Euler(startAngles+(eulerAngles * newValue));
            yield return null;
        }
        go.transform.rotation = Quaternion.Euler(startAngles + eulerAngles);
    }

    //TODO: NOT TESTED AT ALL:
    //public static IEnumerator rotateObjectAround(GameObject go, Vector3 eulerAngles, Vector3 axis, Vector3 direction,float duration, INTERPOLATION_TYPE type = INTERPOLATION_TYPE.SMOOTH, int power = 1)
    //{
    //    Vector3 startAngles = go.transform.eulerAngles;
    //    System.Func<float, float> interpolationFunc = getInterpolationFunc(type);
    //    float newAngle = 0.0f;
    //    float oldAngle = 0.0f;
    //    float deltaAngle = 0.0f;
    //    for (float i = 0; i < duration; i += Time.deltaTime) //might be buggy. not tested super much
    //    {
    //        float pd = i / duration;
    //        newAngle = interpolationFunc(pd);
    //        newAngle = Mathf.Pow(newAngle, power);
    //        deltaAngle = newAngle - oldAngle;
    //        oldAngle = newAngle;
    //        go.transform.RotateAround(axis, direction, deltaAngle);
    //        //go.transform.eulerAngles = startAngles + (eulerAngles * newValue);
    //        yield return 0;
    //    }
    //    //go.transform.eulerAngles = startAngles + (eulerAngles); //TOOD: not sure how to code this to make sure it's precise at the end.
    //}

    public static int findMask(string[] layers, bool flipped = false)
    {
        int layerMask = 0;
        foreach (var layer in layers)
        {
            layerMask += 1 << LayerMask.NameToLayer(layer);
        }
        return flipped ? ~layerMask : layerMask;
    }
}
