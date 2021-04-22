using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShadowWalkSkillData", menuName = "ScriptableObject/ShadowWalkSkillData")]
public class ShadowWalkSkillData : SkillDataBase
{
    [SerializeField]
    private GameObject _shadowLumpGameObject;

    public GameObject ShadowLumpGameObject => _shadowLumpGameObject;

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new ShadowWalkSkillController(skillObject, this, unit);
    }
}
