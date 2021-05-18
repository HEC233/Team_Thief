using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillDataBase : ScriptableObject
{
    [SerializeField, Tooltip("스킬 사용 가능 횟수")] 
    private int _numberOfTimesTheSkill; // 스킬 사용 가능 횟수.
    public int NumberOfTimesTheSkill => _numberOfTimesTheSkill;
    
    [SerializeField]
    private float _coolTime;

    public float CoolTime => _coolTime;

    [SerializeField] 
    private string _skillName;

    public string SkillName => _skillName;

    [SerializeField] 
    private float _increaseEncroachment;

    public float IncreaseEncroachment => _increaseEncroachment;

    [SerializeField] 
    private float _encroachmentDecrease;

    public float DecreaseEncroachment => _encroachmentDecrease;

    // Unit까지 넣어주자.
    public abstract SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit);
}
