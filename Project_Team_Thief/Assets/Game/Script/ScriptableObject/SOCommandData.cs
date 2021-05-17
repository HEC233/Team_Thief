using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommandData", menuName = "ScriptableObject/CommandData")]
public class SOCommandData : ScriptableObject
{
    public string commandString;
    public string reverseCommandString;
    public float coolTime;
    public float maxCoolTIme;
    
    // String을 Key로 전달할 까 아니면 스킬 데이터를 전달할까
    public string skillName;
}
