using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommandData", menuName = "ScriptableObject/CommandData")]
public class SOCommandData : ScriptableObject
{
    public string commandString;
    public string reverseCommandString;
}
