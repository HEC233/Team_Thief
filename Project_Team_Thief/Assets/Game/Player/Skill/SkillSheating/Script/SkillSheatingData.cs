using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSheatingData", menuName = "ScriptableObject/SkillSheatingData")]
public class SkillSheatingData : SkillDataBase
{
    [SerializeField] 
    private float _addCriticalAmount;
    public float AddCriticalAmount => _addCriticalAmount;
    [SerializeField] 
    private float _addCriticalTime;
    public float AddCriticalTime => _addCriticalTime;
    
    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillSheatingController(skillObject, this, unit);
    }
}
