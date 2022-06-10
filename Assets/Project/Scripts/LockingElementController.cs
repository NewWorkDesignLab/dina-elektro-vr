using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockingElementController : MonoBehaviour
{
    public Sicherung fuse;
    public bool lockable;
    public GameObject lockingElement;
    public GameObject lockingElementHolo;
    public GameObject lockParticles;
    Renderer[] renderers;
    List<Material> materials = new List<Material>();
    List<MaterialPropertyBlock> blocks = new List<MaterialPropertyBlock>();
    //[Range(0, 1)]
    //public float holoAmount;
    //float _holoAmount;

    void Start()
    {
        lockingElement.SetActive(false);
        lockingElementHolo.SetActive(false);

        renderers = lockingElementHolo.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < renderers[i].materials.Length; j++)
            {
                blocks.Add(new MaterialPropertyBlock());

                //if (!materials.Contains(renderers[i].materials[j]))
                //{
                //    materials.Add(renderers[i].materials[j]);
                //    blocks.Add(new MaterialPropertyBlock());
                //}
            }
        }
        lockParticles.SetActive(false);

        //UpdateMaterials();
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("LockingElement"))
    //    {
    //        lockable = true;
    //        lockingElementHolo.SetActive(lockable);
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("LockingElement"))
    //    {
    //        lockable = false;
    //        lockingElementHolo.SetActive(lockable);
    //    }
    //}

    public void ShowLockable()
    {
        lockable = true;
        lockingElementHolo.SetActive(true);
    }
    public void HideLockable()
    {
        if(!fuse.locked)
            lockingElementHolo.SetActive(false);
    }

    public void Lock(Finger finger)
    {
        if (!fuse.locked)
        {
            lockParticles.SetActive(true);
            fuse.locked = true;
            StartCoroutine(fuse.VibrateCoroutine(1, finger.controller, 0.5f, true));
        }
    }
    public void Unlock()
    {
        fuse.locked = false;
    }

    void Update()
    {
        //if (_holoAmount != holoAmount)
        //{
        //    UpdateMaterials();
        //    _holoAmount = holoAmount;
        //}
    }
    public void UpdateMaterials(float holoAmount)
    {
        if (fuse.locked)
            return;

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].SetFloat("_HoloAmount", holoAmount);
            renderers[i].SetPropertyBlock(blocks[i]);
        }
    }
}