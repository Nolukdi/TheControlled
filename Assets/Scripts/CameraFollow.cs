using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Target
    public Transform target;
    public float difference = 1;
    public float upperLim = 3;
    public float lowerLim = -2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If the camera is not past limits
        if (target.position.x < upperLim && target.position.x > lowerLim)
        {
            gameObject.transform.position = new Vector3(target.position.x - difference, gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }
}
