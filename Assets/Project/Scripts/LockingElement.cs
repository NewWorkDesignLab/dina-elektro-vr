using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LockingElement : MonoBehaviour
{
    public Transform origin;
    public LayerMask layerMask;
    public Finger finger;
    
    void Start()
    {
        gameObject.SetActive(false);
    }

    LockingElementController currentController;
    LockingElementController lastController;

    private void FixedUpdate()
    {
        RaycastHit[] hits = Physics.SphereCastAll(origin.position, 0.010f, origin.position, 0.010f, layerMask);
        currentController = null;
        LockingElementController closest = null;
        //bool first = false;
        closest = hits.Length > 0 ? hits[0].collider.GetComponent<LockingElementController>() : null;
        float distance= hits.Length > 0 ? Vector3.Distance(closest.lockingElementHolo.transform.position, origin.position) : 0;

        for (int i = 0; i < hits.Length; i++)
        {
            //if (closest == null)
            //{
            //    first = true;
            //    closest = hits[i].collider.GetComponent<LockingElementController>();
            //}
            if (Vector3.Distance(hits[i].transform.position, origin.position) <
                Vector3.Distance(closest.transform.position, origin.position))
            {
                closest = hits[i].collider.GetComponent<LockingElementController>();
                distance = Vector3.Distance(closest.transform.position, origin.position);
            }
        }
        if (closest != null)
        {
            currentController = closest;

            if (currentController != lastController)
                currentController.ShowLockable();

            float placeDistance= 0.01f;

            float remap = Mathf.Clamp(Remap(distance, placeDistance, 0.05f, 0, 1),0,1);
            currentController.UpdateMaterials(remap);
            if(distance< placeDistance)
            {
                currentController.Lock(finger);
                gameObject.SetActive(false);
                finger.animator.SetLayerWeight(3, 0);
            }

            if (lastController != null && lastController != currentController)
                lastController.HideLockable();

            lastController = currentController;
            return;
        }
        if (lastController != null)
        {
            lastController.HideLockable();
            lastController = null;
        }
    }
    public static float Remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }
}
