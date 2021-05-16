using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit",menuName = "ScriptableObject/Unit")]
public class SOUnit : ScriptableObject
{
    public string unitName;
    [Space(8)]
    public float minSpeed;
    public float maxSpeed;

    [Space(8)]
    public float minJumpPower;
    public float maxJumpPower;
    public int jumpCount;

    [Space(8)]
    public float hp;
    public float reduceHit;
    public float knockbackMultiply;

    [Space(8)]
    public float skillDamage;
    public float criticalValue;
    public int attackCount;
    public float attackInterval;
    public float enterDelay;
    public float endDelay;

    [Space(8)]
    public float coolTime;
    public Vector2 knockback;

    [Space(8)]
    public float attackMoveX;
    public float attackMoveXTime;
    public float attackMoveY;
    public float attackMoveYTime;

    [Space(8)]
    public float cameraShakeIntensity;
    public int cameraShakeCount;
    public float hitstopLength;
    public float bulletTimeLength;
}
