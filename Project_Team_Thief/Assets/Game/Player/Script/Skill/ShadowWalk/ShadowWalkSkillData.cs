using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowWalkSkillData : SkillDataBase
{
    [SerializeField]
    private GameObject _shadowLumpGameObject;

    public GameObject ShadowLumpGameObject => _shadowLumpGameObject;

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject)
    {
        return new ShadowWalkSkillController(skillObject, this);
    }
}
