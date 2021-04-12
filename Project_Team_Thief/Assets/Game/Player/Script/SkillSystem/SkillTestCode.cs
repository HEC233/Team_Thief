using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTestCode : MonoBehaviour
{
    [SerializeField]
    private GameSkillMgr _skillMgr = null;

    [SerializeField]
    private SkillDataBase _skillData = null;

    public void InvokeSkill()
    {
        var skillObject = _skillMgr.GetSkillObject();
        if (skillObject == null)
            return;

        skillObject.InitSkill(_skillData.GetSkillController(skillObject)); 
    }
}
