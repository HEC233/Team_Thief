using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObject/SkillData")]
public class SOSkillData : ScriptableObject
{
    public string skillName;

    public string skillCommand;
}


// SO 이중 클래스 각 스킬을 객체들이 각자 알아서 키 처리를 false true