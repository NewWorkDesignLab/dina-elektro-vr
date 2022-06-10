using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum FuseState
{
    On,
    Off
}
public enum FuseType
{
    Default,
    RCCB1,
    RCCB2
}
[System.Serializable]
public class FuseElement
{
    public string id;
    //public FuseType fuseType;
    public Sicherung[] possibleFuses;
    [HideInInspector]
    public Sicherung fuse;
    public bool isOverwritten;
    public FuseState state;
    public UnityEvent turnOnEvent;
    public UnityEvent turnOffEvent;
    public string[] overrideFuses;
    [HideInInspector]
    public List<FuseElement> overwrittenByFuses = new List<FuseElement>();

    public void TurnOn()
    {
        state = FuseState.On;
        OverrideFuses();

        if (!isOverwritten)
            turnOnEvent.Invoke();
    }
    public void TurnOff()
    {
        state = FuseState.Off;
        OverrideFuses();

        //if (!isOverwritten)
            turnOffEvent.Invoke();
    }

    void OverrideFuses()
    {
        if (overrideFuses.Length == 0)
            return;
        for (int i = 0; i < overrideFuses.Length; i++)
        {
            FuseElement fuseElement = null;
            for (int j = 0; j < FuseManager.instance.fuseElements.Length; j++)
                if (FuseManager.instance.fuseElements[j].id == overrideFuses[i])
                    fuseElement = FuseManager.instance.fuseElements[j];

            if (fuseElement == null)
            {
                Debug.LogError($"FUSE '{overrideFuses[i]}' NOT FOUND");
                return;
            }

            if (!fuseElement.overwrittenByFuses.Contains(this))
            {
                fuseElement.overwrittenByFuses.Add(this);
            }
            fuseElement.isOverwritten = false;

            if (fuseElement.state == FuseState.On && state == FuseState.On)
            {
                fuseElement.turnOnEvent.Invoke();
            }
            else if(state == FuseState.Off)
            {
                fuseElement.isOverwritten = true;
                fuseElement.turnOffEvent.Invoke();
            }
        }
    }
}
public class FuseManager : MonoBehaviour
{
    public static FuseManager instance;
    public FuseElement[] fuseElements;
    public List<Sicherung> fuses;
    List<Sicherung> remainingFuses;

    private void Awake()
    {
        remainingFuses = new List<Sicherung>(fuses);
        instance = this;
        for (int i = 0; i < fuseElements.Length; i++)
        {
            FuseElement fuseElement = fuseElements[i];
            //fuseElement.turnOnEvent.AddListener(fuseElement.TurnOn);
            //fuseElement.turnOffEvent.AddListener(fuseElement.TurnOff);

            List<Sicherung> possibleFuses=new List<Sicherung>();
            for (int j = 0; j < fuseElement.possibleFuses.Length; j++)
            {
                if (remainingFuses.Contains(fuseElement.possibleFuses[j]))
                    possibleFuses.Add(fuseElement.possibleFuses[j]);
            }
            if(possibleFuses.Count==0)
            {
                Debug.LogError($"FUSE '{fuseElement.id}' ALREADY TAKEN OR NOT FOUND");
                return;
            }
            Sicherung fuse = possibleFuses[Random.Range(0, possibleFuses.Count)];
            remainingFuses.Remove(fuse);
            fuseElement.fuse = fuse;
            fuse.turnOnEvent.AddListener(fuseElement.TurnOn);
            fuse.turnOffEvent.AddListener(fuseElement.TurnOff);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
