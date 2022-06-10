using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SicherungCollider : MonoBehaviour
{
    Sicherung sicherung;
    void Awake()
    {
        sicherung = GetComponentInParent<Sicherung>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Finger"))
            sicherung.TriggerEnter(other.GetComponent<Finger>(), transform.position);
    }
    private void OnTriggerExit(Collider other)
    {
        sicherung.TriggerExit();
    }
}
