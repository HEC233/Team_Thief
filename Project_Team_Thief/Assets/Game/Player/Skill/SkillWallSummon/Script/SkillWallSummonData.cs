using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillWallSummon", menuName = "ScriptableObject/SkillWallSummonData")]
public class SkillWallSummonData : SkillDataBase
{
    [SerializeField] 
    private GameObject _wallSummonGO;

    public GameObject WallSummonGO => _wallSummonGO;


    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillWallSummonController(skillObject, this, unit);
    }
}
