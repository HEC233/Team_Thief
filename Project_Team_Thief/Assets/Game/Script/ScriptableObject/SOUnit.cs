using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit",menuName = "ScriptableObject/Unit")]
public class SOUnit : ScriptableObject
{
    public string unitName;

    public float minSpeed;
    public float maxSpeed;

    public float minJumpPower;
    public float maxJumpPower;
    public int jumpCount;

    public float hp;
    public float reduceHit;

    public float skillDamage;
    public float criticalValue;
    public int attackCount;
    public float attackInterval;
    public float enterDelay;
    public float endDelay;

    public float coolTime;
    public float knockbackDist;
    public float knockbackTime;

    public float attackMoveX;
    public float attackMoveXTime;
    public float attackMoveY;
    public float attackMoveYTime;

    public float cameraShakeIntensity;
    public int cameraShakeCount;
    public float hitstopLength;
    public float bulletTimeLength;
}
