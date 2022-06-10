using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sicherung : MonoBehaviour
{
    public Transform Schalter;
    public float onAngle = 0;
    public float offAngle = 90;
    public float resistanceAngle = 10;
    float _currentAngle;
    public FuseState state;
    public FuseState _state;
    //public bool on;
    Transform colliderTransform;
    public Finger currentFinger;
    bool importantFeedback;

    public UnityEvent turnOnEvent;
    public UnityEvent turnOffEvent;
    public bool locked;
    void Start()
    {
    }
    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < 90 || angle > 270)
        {       // if angle in the critic region...
            if (angle > 180) angle -= 360;  // convert all angles to -180..+180
            if (max > 180) max -= 360;
            if (min > 180) min -= 360;
        }
        angle = Mathf.Clamp(angle, min, max);
        if (angle < 0) angle += 360;  // if angle negative, convert to 0..360
        return angle;
    }
    void Update()
    {
        if (locked)
            return;

        if (colliderTransform != null)
        {
            StartCoroutine(VibrateCoroutine(0.15f, currentFinger.controller, 0.1f));
            Schalter.LookAt(colliderTransform);
            Schalter.localRotation = Quaternion.Euler(ClampAngle(Clamp0360(Schalter.localRotation.eulerAngles.x + 45), onAngle, offAngle), 0, 0); ;
        }
        if (_currentAngle > resistanceAngle && Schalter.rotation.eulerAngles.x <= resistanceAngle
            || _currentAngle < resistanceAngle && Schalter.rotation.eulerAngles.x >= resistanceAngle)
        {
            StartCoroutine(VibrateCoroutine(1, currentFinger.controller, 0.2f, true));
        }
        _currentAngle = Schalter.rotation.eulerAngles.x;
        state = _currentAngle < resistanceAngle ? FuseState.On : FuseState.Off;

        if (_state != state)
        {
            if (state == FuseState.On)
                turnOnEvent.Invoke();
            else
                turnOffEvent.Invoke();

            _state = state;
        }
    }

    IEnumerator RotateCoroutine(float targetAngle, float duration)
    {
        float currentTime = 0;
        float startAngle = Schalter.rotation.eulerAngles.x;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            Schalter.localRotation = Quaternion.Euler(
                Mathf.Lerp(startAngle, targetAngle, currentTime / duration),
                Schalter.localRotation.y,
                Schalter.localRotation.z);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator VibrateCoroutine(float intensity, OVRInput.Controller controller, float duration, bool overrideVibration=false)
    {
        if (!importantFeedback)
        {
            if (overrideVibration)
            {
                importantFeedback = true;
                Debug.Log("OVERRDIDE");
            }
            OVRInput.SetControllerVibration(1, intensity, controller);
            yield return new WaitForSecondsRealtime(duration);
            OVRInput.SetControllerVibration(0, 0, controller);
            if (overrideVibration)
                importantFeedback = false;
        }
    }

    public void TriggerEnter(Finger finger, Vector3 colliderPosition)
    {
        currentFinger = finger;
        colliderTransform = finger.GetClosestInteractionPoint(colliderPosition);
    }
    public void TriggerExit()
    {
        colliderTransform = null;
        StartCoroutine(RotateCoroutine(state == FuseState.On ? onAngle : offAngle, 0.1f));
    }
}
