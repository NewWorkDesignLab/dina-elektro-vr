using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomHandController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        float isPointingLeft = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        bool isPointingRight = OVRInput.Get(OVRInput.NearTouch.Any);
        Debug.Log("left " + isPointingLeft);
        Debug.Log("right " + isPointingRight);

        bool any = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        Debug.Log("any " + any);
    }
}
