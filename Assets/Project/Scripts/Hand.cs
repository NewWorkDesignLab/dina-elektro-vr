using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    Animator animator;
    //SkinnedMeshRenderer mesh;
    private float gripTarget;
    //private bool indexTarget;
    //private bool indexCurrent;
    private bool index, _index, thumb, _thumb;
    private float triggerTarget;
    private float gripCurrent;
    private float triggerCurrent;


    public float speed;

    private string ANIMATOR_GRIP = "Grip";
    private string ANIMATOR_TRIGGER = "Trigger";
    private string ANIMATOR_INDEX = "Point Layer";
    private string ANIMATOR_THUMB = "Thumb Layer";
    //private OVRInput.Controller m_controller = OVRInput.Controller.LTouch;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        //bool isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger);
        //bool isPointingLeft = OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
        //bool isPointingRight = OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        //Debug.Log("left " + isPointingLeft);
        //Debug.Log("right " + isPointingRight);
        AnimateHand();
    }

    internal void SetTrigger(float v)
    {
        animator.SetFloat("Pinch", v);
        //animator.SetFloat("Grip", v);
        //triggerTarget = v;
    }

    internal void SetGrip(float v)
    {
        animator.SetFloat("Flex", v);
        //animator.SetFloat("Pose", v);
        //gripTarget = v;
    }

    internal void SetIndex(bool v)
    {
        if (v != _index)
        {
            //indexTarget = v;
            //animator.SetLayerWeight(2, v ? 0 : 1);
            indexTouchCount++;
            StartCoroutine(BlendLayerCoroutine(ANIMATOR_INDEX, v ? 0 : 1, 0.2f, indexTouchCount));
            _index = v;
        }
    }
    int thumbTouchCount;
    int indexTouchCount;
    internal void SetThumb(bool v)
    {
        if (v != _thumb)
        {
            thumbTouchCount++;
            StartCoroutine(BlendLayerCoroutine(ANIMATOR_THUMB, v ? 0 : 1, 0.2f, thumbTouchCount));
            _thumb = v;
        }
    }

   

    IEnumerator BlendLayerCoroutine(string layerName, float target, float duration, int id)
    {
        int layer = animator.GetLayerIndex(layerName);
        float currentTime = 0;
        float start = animator.GetLayerWeight(layer);

        bool end = false;
        if (layerName == ANIMATOR_INDEX && id != indexTouchCount)
            end = true;
        if (layerName == ANIMATOR_THUMB && id != thumbTouchCount)
            end = true;

        while (currentTime < duration && !end)
        {
            currentTime += Time.deltaTime;
            animator.SetLayerWeight(layer, Mathf.Lerp(start, target, currentTime / duration));
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume, bool destroy = true)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (destroy)
            Destroy(audioSource.gameObject);
        yield break;
    }

    void AnimateHand()
    {
        if (gripCurrent != gripTarget)
        {
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * speed);
            animator.SetFloat(ANIMATOR_GRIP, gripCurrent);
        }

        if (triggerCurrent != triggerTarget)
        {
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * speed);
            animator.SetFloat(ANIMATOR_TRIGGER, triggerCurrent);
        }

        //animator.SetFloat(ANIMATOR_INDEX, indexTarget ? 1 : 0);
    }

    public void ToggleVisibility()
    {
        //mesh.enabled = !mesh.enabled;
    }
}
