using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "ShadowWalkSkillData", menuName = "ScriptableObject/SkillData/Old/ShadowWalkSkillData")]
public class ShadowWalkSkillData : SkillDataBase
{
    [SerializeField]
    private GameObject _shadowLumpGameObject;

    public GameObject ShadowLumpGameObject => _shadowLumpGameObject;
    
    [SerializeField]
    private float _shadowLumpAmount = 0.0f;
    public float ShadowLumpAmount => _shadowLumpAmount;

    [SerializeField]
    private float _controlTime = 0.0f;

    public float ControlTime => _controlTime;

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new ShadowWalkSkillController(skillObject, this, unit);
    }
}
