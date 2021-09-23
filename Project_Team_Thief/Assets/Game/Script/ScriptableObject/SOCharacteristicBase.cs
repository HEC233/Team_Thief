using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOCharacteristicBase : MonoBehaviour
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
    
    [SerializeField]
    protected Sprite _spriteImage;
    public Sprite SpriteImage => _spriteImage;

    [SerializeField] 
    protected float _probability;
    public float Probability => _probability;

    [SerializeField] 
    protected float _originalProbability;
    public float OriginalProbability => _originalProbability;

    public abstract void ActiveCharacteristic(Unit unit);

    public abstract void SetContentString();

    public virtual void RoadSpriteImage()
    {
        _spriteImage = Addressable.instance.GetSprite(_spriteImageName);
    }
}
