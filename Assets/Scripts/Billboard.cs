using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        //Vector3 newPos = transform.position;
        //newPos.z = -(transform.rotation.x / 90.0f);
        //transform.position = newPos;
    }
}