using MultiUserKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class TeleportTargetAnimator : MonoBehaviour
{
    Material material;
    public float speed;
    public float maxFadeScale=1;
    public float minFadeScale=0.2f;
    void Start()
    {
        material = GetComponent<MeshRenderer>().sharedMaterial;
    }
    void Update()
    {
        if (Raycaster.instance.raycastInstances[2].active || Raycaster.instance.raycastInstances[3].active)
            material.SetFloat("_FadeMapScale", minFadeScale + ((Mathf.Sin(Time.time * speed) + 1) / 2) * (maxFadeScale - minFadeScale));
        else
            material.SetFloat("_FadeMapScale", 0);
    }
}
