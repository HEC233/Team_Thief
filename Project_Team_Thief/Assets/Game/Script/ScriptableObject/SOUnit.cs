using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit",menuName = "ScriptableObject/Unit")]
public class SOUnit : ScriptableObject
{
    [SerializeField]
    private string _unitName;
    [Space(8)]
    [SerializeField]
    private float _minSpeed;
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _accelation;

    [Space(8)]
    [SerializeField]
    private float _minJumpPower;
    [SerializeField]
    private float _maxJumpPower;
    [SerializeField]
    private int _jumpCount;

    [Space(8)]
    [SerializeField]
    private float _hp;
    [SerializeField]
    private float _reduceHit;
    [SerializeField]
    private float _knockbackMultiply;

    [Space(8)]
    [SerializeField]
    private float _attackDamage;
    [SerializeField]
    private float _criticalValue;
    [SerializeField]
    private int _attackCount;
    [SerializeField]
    private float _attackInterval;
    [SerializeField]
    private float _enterDelay;
    [SerializeField]
    private float _endDelay;

    [Space(8)]
    [SerializeField]
    private float _coolTime;
    [SerializeField]
    private Vector2 _knockback;

    [Space(8)]
    [SerializeField]
    private float _attackMoveX;
    [SerializeField]
    private float _attackMoveXTime;
    [SerializeField]
    private float _attackMoveY;
    [SerializeField]
    private float _attackMoveYTime;

    [Space(8)]
    [SerializeField]
    private float _cameraShakeIntensity;
    [SerializeField]
    private int _cameraShakeCount;
    [SerializeField]
    private float _hitstopLength;
    [SerializeField]
    private float _bulletTimeLength;

    [Space(8)]
    [SerializeField]
    private float _lightFragProbablity;
    [SerializeField]
    private int _lightFragAmountMin;
    [SerializeField]
    private int _lightFragAmountMax;

    public string unitName => _unitName;
    public float minSpeed => _minSpeed;
    public float maxSpeed => _maxSpeed;
    public float accelation => _accelation;
    public float minJumpPower => _minJumpPower;
    public float maxJumpPower => _maxJumpPower;
    public int jumpCount => _jumpCount;
    public float hp => _hp;
    public float reduceHit => _reduceHit;
    public float knockbackMultiply => _knockbackMultiply;
    public float attackDamage => _attackDamage;
    public float criticalValue => _criticalValue;
    public int attackCount => _attackCount;
    public float attackInterval => _attackInterval;
    public float enterDelay => _enterDelay;
    public float endDelay => _endDelay;
    public float coolTime => _coolTime;
    public Vector2 knockback => _knockback;
    public float attackMoveX => _attackMoveX;
    public float attackMoveXTime => _attackMoveXTime;
    public float attackMoveY => _attackMoveY;
    public float attackMoveYTime => _attackMoveYTime;
    public float cameraShakeIntensity => _cameraShakeIntensity;
    public int cameraShakeCount => _cameraShakeCount;
    public float hitstopLength => _hitstopLength;
    public float bulletTimeLength => _bulletTimeLength;
    public float LightFragProbablity => _lightFragProbablity;
    public int LightFragAmountMin => _lightFragAmountMin;
    public int LightFragAmountMax => _lightFragAmountMax;
}
