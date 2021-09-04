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
    
    [SerializeField] 
    protected string originalContentString;
    public string OriginalContentString => originalContentString;

    [SerializeField]
    protected string originalDurationString;
    public string OriginalDurationString => originalDurationString;

    [SerializeField]
    protected string _spriteImageName;
    public string SpriteImageName => _spriteImageName;

    public abstract void ActivePenalty(Unit unit);

    public abstract void SetContentString();

    public abstract void SetAddPenalty(float zeroTimer);

}
