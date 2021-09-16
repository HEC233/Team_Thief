using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillChaosHallData", menuName = "ScriptableObject/SkillData/SkillChaosHallData")]
public class SkillChaosHallData : SkillDataBase
{
    [SerializeField] 
    private float _bulletTimeScale;
    public float BulletTimeScale => _bulletTimeScale;

    [SerializeField] 
    private float _bulletTimeAmount;
    public float BulletTimeAmount => _bulletTimeAmount;
    
    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillChaosHallController(skillObject, this, unit);
    }
}
