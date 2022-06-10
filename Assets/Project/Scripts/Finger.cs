using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : MonoBehaviour
{
    [HideInInspector]
    public OVRInput.Controller controller;
    public Transform[] interactionPoints;
    public Animator animator;
    void Awake()
    {
        controller = GetComponentInParent<TouchControllerTracking>().Controller;
    }

    public Transform GetClosestInteractionPoint(Vector3 anchor)
    {
        Transform closestPoint = interactionPoints[0];
        for (int i = 0; i < interactionPoints.Length; i++)
        {
            if (Vector3.Distance(anchor, interactionPoints[i].position) < Vector3.Distance(anchor, closestPoint.position))
                closestPoint = interactionPoints[i];
        }
        return closestPoint;
    }
}
