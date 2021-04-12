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

    public event UnityAction OnEndSkill = null;

    public SkillControllerBase(GameSkillObject skillObject, SkillDataBase data)
    {
        _skillObject = skillObject;
        _skillData = data;
    }

    public abstract void Invoke();

    public virtual void Release()
    {
        _skillData = null;
        _skillObject = null;
        OnEndSkill = null;
    }
}
