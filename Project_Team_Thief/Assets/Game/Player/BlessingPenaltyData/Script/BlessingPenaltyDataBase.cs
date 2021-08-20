using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlessingPenaltyDataBase : ScriptableObject
{
    [SerializeField]
    protected int _id;
    public int ID => _id;
    
    [SerializeField] 
    protected string _name;
    public string Name => _name;

    [SerializeField]
    protected string durationString;
    public string DurationString => durationString;

    [SerializeField] 
    protected string contentString;
    public string ContentString => contentString;

    public abstract void ActivePenalty(Unit unit);

    protected abstract void SetContentString();
}
