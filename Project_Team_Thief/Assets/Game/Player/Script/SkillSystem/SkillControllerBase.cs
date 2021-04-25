using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SkillControllerBase
{
    private GameSkillObject _skillObject = null;
    public GameSkillObject SkillObject => _skillObject;

    private SkillDataBase _skillData = null;
    public SkillDataBase SkillData => _skillData;

    private Unit _unit;

    public Unit Unit => _unit;

    protected UnityAction OnEndSkillAction = null;   // todo Event vs evnet UnityAction?
    public event UnityAction OnEndSkillEvent
    {
        add => OnEndSkillAction += value;
        remove => OnEndSkillAction -= value;
    }
    
    public SkillControllerBase(GameSkillObject skillObject, SkillDataBase data, Unit unit)
    {
        _skillObject = skillObject;
        _skillData = data;
        _unit = unit;
    }

    public abstract void Invoke();

    public virtual void Release()
    {
        _skillData = null;
        _skillObject = null;
        OnEndSkillAction = null;
    }
}
