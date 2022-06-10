using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControllerTracking : MonoBehaviour
{

    //Public Buttons
    public bool buttonOnePress = false;
    public bool buttonTwoPress = false;
    public bool buttonStartPress = false;
    public bool buttonStickPress = false;

    //Public Capacitive Touch
    public bool thumbRest = false;
    public bool buttonOneTouch = false;
    public bool buttonTwoTouch = false;
    public bool buttonThreeTouch = false;
    public bool buttonFourTouch = false;
    public bool buttonTrigger = false;
    public bool buttonStick = false;

    //Public Near Touch
    public bool nearTouchIndexTrigger = false;
    public bool nearTouchThumbButtons = false;

    //Public Trigger & Grip
    public float trigger = 0.0f;
    public float grip = 0.0f;

    //Public Stick Axis
    Vector2 stickXYPos;
    public float stickXPos = 0.0f;
    public float stickYPos = 0.0f;

    //Public Define Controller (Left/Right)
    public OVRInput.Controller Controller;


    // Update is called once per frame
    void Update()
    {

        //Controller Position & Rotation
        //transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        //transform.localRotation = OVRInput.GetLocalControllerRotation(Controller);

        //Controller Button State
        buttonOnePress = OVRInput.Get(OVRInput.Button.One, Controller);
        buttonTwoPress = OVRInput.Get(OVRInput.Button.Two, Controller);
        buttonStartPress = OVRInput.Get(OVRInput.Button.Start, Controller);
        buttonStickPress = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, Controller);

        //Controller Capacitive Sensors State
        thumbRest = OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, Controller);
        buttonOneTouch = OVRInput.Get(OVRInput.Touch.One, Controller);
        buttonTwoTouch = OVRInput.Get(OVRInput.Touch.Two, Controller);
        buttonThreeTouch = OVRInput.Get(OVRInput.Touch.Three, Controller);
        buttonFourTouch = OVRInput.Get(OVRInput.Touch.Four, Controller);
        buttonTrigger = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, Controller);
        buttonStick = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, Controller);

        //Controller NearTouch State
        nearTouchIndexTrigger = OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, Controller);
        nearTouchThumbButtons = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, Controller);

        //Controller Trigger State
        grip = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller);
        trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, Controller);

        //Controller Analogue Stick State
        stickXYPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, Controller);
        stickXPos = stickXYPos.x;
        stickYPos = stickXYPos.y;


        //Output Logs
        //  

    }
}