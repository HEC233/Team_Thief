using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "SkillPlainSwordData", menuName = "ScriptableObject/SkillData/Old/SkillPlainSwordData")]

public class SkillPlainSwordData : SkillDataBase
{
    [SerializeField]
    private SignalSourceAsset[] _cinemachineSignalSourceArr;
    public SignalSourceAsset[] CinemachineSignalSourceArr => _cinemachineSignalSourceArr;
    
    [SerializeField] 
    private Vector2[] _knockBackPowerArr;
    public Vector2[] KnockBackPowerArr => _knockBackPowerArr;
    
    [SerializeField]
    private float[] _attackDamageArr;
    public float[] AttackDamageArr => _attackDamageArr;

    [SerializeField]
    private float _multiStateHitInterval;
    public float MultiStateHitInterval => _multiStateHitInterval;

    [SerializeField]
    private float _multiStateHitIntervalFastAmount;

    public float MultiStateHitIntervalFastAmount => _multiStateHitIntervalFastAmount;

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillPlainSwordController(skillObject, this, unit);
    }
}
