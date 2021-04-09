using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObject/SkillData")]
public class SOSkillData : ScriptableObject
{
    public string skillName;

    public string skillCommand;
}
