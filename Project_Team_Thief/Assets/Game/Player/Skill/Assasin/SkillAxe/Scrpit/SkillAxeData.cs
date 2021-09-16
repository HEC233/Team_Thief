using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "SkillAxeData", menuName = "ScriptableObject/SkillData/SkillAxeData")]
public class SkillAxeData : SkillDataBase
{
    [SerializeField] 
    private GameObject _axeGameObject;
    public GameObject AxeGameObject => _axeGameObject;

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillAxeController(skillObject, this, unit);
    }
}
