using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSkill : NPCController
{
    private bool _acted = false;
    public override bool Act()
    {
        if (_acted)
        {
            return false;
        }

        GameManager.instance.UIMng.StartSkillSelect();

        _acted = true;
        return true;
    }
}
