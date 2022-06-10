using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//[RequireComponent(typeof(ActionBasedController))]
public class HandController : MonoBehaviour
{
    //ActionBasedController controller;
    public Hand hand;
    public TouchControllerTracking ovrInput;

    //InputAction valueAction = inputActionAsset.FindAction(inputTrigger.ToString() + "-Value", true);
    //valueAction.performed += ctx => UpdateValue(inputTrigger, valueAction);
    //triggers[inputTrigger].value = valueAction.ReadValue<float>();
    // Start is called before the first frame update
    void Start()
    {
        //controller = GetComponent<ActionBasedController>();
    }

    // Update is called once per frame
    void Update()
    {
        hand.SetGrip(ovrInput.grip);
        hand.SetTrigger(ovrInput.trigger);
        hand.SetIndex(ovrInput.buttonTrigger);
        hand.SetThumb(ovrInput.buttonOneTouch);
        hand.SetGrip(ovrInput.grip);
    }
}
