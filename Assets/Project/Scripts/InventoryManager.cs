using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiUserKit;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager leftInventoryManager;
    public static InventoryManager rightInventoryManager;
    public enum Hand
    {
        LeftHand,
        RightHand,
    }
    [System.Serializable]
    public class InventoryItem
    {
        public GameObject itemPrefab;
        public int layerIndex;
        public string animation;
        public bool bothHandsNeeded;
        public GameObject[] itemObjects;
        public UnityEvent onActivate;
    }
    public Hand hand;
    public Animator leftHandAnimator;
    public Animator rightHandAnimator;
    public InventoryItem[] items;
    //public int itemCount;
    public float pieScale=0.5f;
    public GameObject PieParts;
    public Material enterMaterial;
    public Material exitMaterial;
    public LayerMask layer;
    public LockingElement lockingElement;
    public int openAngle=50;
    public AnimationCurve openCurve;
    public float durationPerPart = 0.5f;
    public float openDelay = 0.2f;
    List<Transform> _items=new List<Transform>();
    int currentItem;
    public static int layerMaskToLayer(LayerMask layerMask)
    {
        int layerNumber = 0;
        int layer = layerMask.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }
    private void Awake()
    {
        if (hand == Hand.LeftHand)
            leftInventoryManager = this;
        else
            rightInventoryManager = this;
    }
    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            GameObject piePart = Instantiate(PieParts.transform.GetChild(items.Length - 1).gameObject, transform.GetChild(0));
            piePart.transform.Rotate(0, i * (360 / items.Length), 0, Space.Self);
            piePart.transform.localScale = new Vector3(0, 0, 0);
            MeshCollider collider = piePart.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.isTrigger = true;
            collider.sharedMesh = collider.GetComponent<MeshFilter>().mesh;
            Interactable button = piePart.AddComponent<Interactable>();
            button.EnterMaterial = enterMaterial;
            button.ExitMaterial = exitMaterial;
           
            button.onPointerClick.AddListener(items[i].onActivate.Invoke);
            button.onPointerClick.AddListener(Select);
            if (piePart.GetComponent<BoxCollider>() != null)
                Destroy(piePart.GetComponent<BoxCollider>());
            piePart.GetComponent<MeshRenderer>().material = exitMaterial;
            piePart.layer = layerMaskToLayer(layer);
            GameObject item = Instantiate(items.Length-1 >= i ? items[i].itemPrefab :items[0].itemPrefab, piePart.transform.GetChild(0));;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = new Vector3(1 / pieScale, 1 / pieScale, 1 / pieScale);
            _items.Add(piePart.transform);

        }

        if (hand == Hand.LeftHand)
            Reset();

        Raycaster.instance.StartIgnoringRaycastInstance(hand == Hand.LeftHand ? "Right Controller Interaction" : "Left Controller Interaction");

    }

    public void UpdateCurrentItem(int id)
    {
        currentItem = id;
    }

    bool opened;
    bool _opened;
    private void Update()
    {
        Vector3 targetDir = LocalUser.cameraTransform.position - transform.position;
        targetDir = targetDir.normalized;

        float dot = Vector3.Dot(targetDir, transform.up);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;


        //transform.GetChild(0).gameObject.SetActive(angle < openAngle);
        opened =angle < openAngle;
        if (opened != _opened)
        {
            if (opened)
            {

                Raycaster.instance.StopIgnoringRaycastInstance(hand == Hand.LeftHand ? "Right Controller Interaction" : "Left Controller Interaction");
            }
            else
            {
                Raycaster.instance.StartIgnoringRaycastInstance(hand == Hand.LeftHand ? "Right Controller Interaction" : "Left Controller Interaction");
            }
            StartCoroutine(ScaleItemsCoroutine(_items.ToArray(), 
                opened ? pieScale : 0, 
                opened ? 0 : 85, 
                opened ? 0 : -0.05f,
                 durationPerPart,
                openDelay));
            _opened = opened;
        }
    }

    static void Reset()
    {
        leftInventoryManager.leftHandAnimator.SetLayerWeight(3, 0);
        leftInventoryManager.leftHandAnimator.SetLayerWeight(4, 0);
        leftInventoryManager.rightHandAnimator.SetLayerWeight(3, 0);
        leftInventoryManager.rightHandAnimator.SetLayerWeight(4, 0);

        for (int i = 0; i < leftInventoryManager.items.Length; i++)
            for (int j = 0; j < leftInventoryManager.items[i].itemObjects.Length; j++)
                leftInventoryManager.items[i].itemObjects[j].SetActive(false);

        for (int i = 0; i < rightInventoryManager.items.Length; i++)
            for (int j = 0; j < rightInventoryManager.items[i].itemObjects.Length; j++)
                rightInventoryManager.items[i].itemObjects[j].SetActive(false);
    }
    void Select()
    {
        Reset();

        if (items[currentItem].itemObjects != null)
        {
            for (int i = 0; i < items[currentItem].itemObjects.Length; i++)
            {
                items[currentItem].itemObjects[i].SetActive(true);
            }
        }
        if (items[currentItem].bothHandsNeeded)
        {
            leftHandAnimator.SetLayerWeight(items[currentItem].layerIndex, 1);
            leftHandAnimator.Play(items[currentItem].animation);
            rightHandAnimator.SetLayerWeight(items[currentItem].layerIndex, 1);
            rightHandAnimator.Play(items[currentItem].animation);
        }
        else
        {
            if (hand == Hand.RightHand)
            {
                leftHandAnimator.SetLayerWeight(items[currentItem].layerIndex, 1);
                leftHandAnimator.Play(items[currentItem].animation);
            }
            if (hand == Hand.LeftHand)
            {
                rightHandAnimator.SetLayerWeight(items[currentItem].layerIndex, 1);
                rightHandAnimator.Play(items[currentItem].animation);
            }
        }
        //lockingElement.gameObject.SetActive(true);
        //lockingElement.finger.animator.SetLayerWeight( 3, 1);

    }

    IEnumerator ScaleItemsCoroutine(Transform[] items, float targetScale, float targetAngle, float targetHeight, float duration, float delay)
    {
        for(int i = 0; i < items.Length; i++)
        {
            StartCoroutine(ScaleItemCoroutine(items[i], targetScale, targetAngle, targetHeight, duration));
            yield return new WaitForSecondsRealtime(openDelay);
        }
    }
    IEnumerator ScaleItemCoroutine(Transform item, float targetScale, float targetAngle, float targetHeight, float duration)
    {
        float currentTime = 0;
        float startScale = item.localScale.x;
        float startAngle = item.localRotation.eulerAngles.x;
        float startHeight = item.localPosition.y;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, targetScale, openCurve.Evaluate(currentTime / duration));
            item.localScale = new Vector3(scale, scale, scale);

            float x = Mathf.Lerp(startAngle, targetAngle, openCurve.Evaluate(currentTime / duration));
            item.localRotation = Quaternion.Euler(x, item.localRotation.eulerAngles.y, item.localRotation.eulerAngles.z);

            float height = Mathf.Lerp(startHeight, targetHeight, openCurve.Evaluate(currentTime / duration));
            item.localPosition = new Vector3(item.localPosition.x, height, item.localPosition.z);

            yield return new WaitForEndOfFrame();
        }
    }
}
