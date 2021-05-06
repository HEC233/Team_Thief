using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "SkillAxeData", menuName = "ScriptableObject/SkillAxeData")]
public class SkillAxeData : SkillDataBase
{
    [SerializeField] 
    private GameObject _axeGameObject;
    public GameObject AxeGameObject => _axeGameObject;
    
    [SerializeField] 
    private SignalSourceAsset _cinemachineSignalSource;
    public SignalSourceAsset CinemachineSignalSource => _cinemachineSignalSource;

    [SerializeField] 
    private Vector2 _knockBackPower;
    public Vector2 KnockBackPower => _knockBackPower;
    
    [SerializeField]
    private float _axeMovePostionX;
    public float AxeMovePostionX => _axeMovePostionX;

    [SerializeField] 
    private float _axeMoveTime;
    public float AxeMoveTime => _axeMoveTime;

    [SerializeField]
    private float _axeAttackDamage;
    public float AxeAttackDamage => _axeAttackDamage;


    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillAxeController(skillObject, this, unit);
    }
}
