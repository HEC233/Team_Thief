using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSpearData", menuName = "ScriptableObject/SkillSpearData")]
public class SkillSpearData : SkillDataBase
{
    private SignalSourceAsset _cinemachineSignalSource;
    public SignalSourceAsset CinemachineSignalSource => _cinemachineSignalSource;

    [SerializeField] 
    private Vector2 _knockBackPower;
    public Vector2 KnockBackPower => _knockBackPower;
    
    [SerializeField]
    private float _playerMovePostionX;
    public float PlayerMovePostionX => _playerMovePostionX;

    [SerializeField] 
    private float _playerMoveTime;
    public float PlayerMoveTime => _playerMoveTime;
    
    [SerializeField]
    private float _attackDamage;
    public float AttackDamage => _attackDamage;
    
    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillSpearController(skillObject, this, unit);
    }
}
