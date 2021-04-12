using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillDataBase : ScriptableObject
{
    public abstract SkillControllerBase GetSkillController(GameSkillObject skillObject);
}
