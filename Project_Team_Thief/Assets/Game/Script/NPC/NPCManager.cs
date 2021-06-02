using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField]
    private NPCController[] npcs;
    private string nearestNpcName;
    private float nearestNpcDist;
    private int nearestNpcIndex = 0;
    private bool bNearestNpcExist = false;

    private Unit controlUnit;

    private void Update()
    {
        controlUnit = GameManager.instance?.GetControlActor()?.GetUnit();
        nearestNpcDist = float.MaxValue;

        if (controlUnit == null)
            return;
        npcs[nearestNpcIndex].ActiveIcon(false); 
        bNearestNpcExist = false;
        for (int i = 0; i < npcs.Length; i++)
        {
            float dist = (npcs[i].transform.position - controlUnit.transform.position).sqrMagnitude;
            if(nearestNpcDist > dist)
            {
                nearestNpcDist = dist;
                nearestNpcIndex = i;
                nearestNpcName = npcs[i].name;
            }
        }

        if(nearestNpcDist <= 9)
        {
            npcs[nearestNpcIndex].ActiveIcon(true);
            bNearestNpcExist = true;
        }
    }

    public string GetNearestNPC()
    {
        if (bNearestNpcExist)
            return nearestNpcName;
        else
            return string.Empty;
    }

}
