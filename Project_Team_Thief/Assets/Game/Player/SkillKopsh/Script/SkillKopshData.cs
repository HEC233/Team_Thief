using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillKopshData", menuName = "ScriptableObject/SkillKopshData")]
public class SkillKopshData : SkillDataBase
{
    [SerializeField] 
    private Vector2[] _knockBackPowerArr;

    public Vector2[] KnockBackPowerArr => _knockBackPowerArr;

    [SerializeField]
    private float[] _attackDamageArr;
    public float[] AttackDamageArr => _attackDamageArr;

    [SerializeField] 
    private float[] _playerMovePostionXArr;
    public float[] PlayerMovePostionXArr => _playerMovePostionXArr;

    [SerializeField]
    private float[] _playerMoveTimeArr;
    public float[] PlayerMoveTimeArr => _playerMoveTimeArr;

    
    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillKopshController(skillObject, this, unit);
    }
}
