using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillHammerData", menuName = "ScriptableObject/SkillHammerData")]
public class SkillHammerData : SkillDataBase
{
    [SerializeField]
    private SignalSourceAsset _cinemachineSignalSource;
    
    public SignalSourceAsset CinemachineSignalSource => _cinemachineSignalSource;
    
    [SerializeField] 
    private Vector2 _knockBackPower;
    public Vector2 KnockBackPower => _knockBackPower;
    
    [SerializeField]
    private float _attackDamage;
    public float AttackDamage => _attackDamage;
    
    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillHammerController(skillObject, this, unit);
    }
}
