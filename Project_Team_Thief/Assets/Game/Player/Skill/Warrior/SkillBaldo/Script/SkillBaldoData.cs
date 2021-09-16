using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillBaldoData", menuName = "ScriptableObject/SkillData/SkillBaldoData")]
public class SkillBaldoData : SkillDataBase
{
    [SerializeField] 
    private float _addCriticalAmount;
    public float AddCriticalAmount => _addCriticalAmount;
    [SerializeField] 
    private float _addCriticalTime;
    public float AddCriticalTime => _addCriticalTime;

    [SerializeField] 
    private float _bulletTimeScale;
    public float BulletTimeScale => _bulletTimeScale;

    [SerializeField] 
    private float _bulletTimeAmount;
    public float BulletTimeAmount => _bulletTimeAmount;

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillBaldoController(skillObject, this, unit);
    }
}
