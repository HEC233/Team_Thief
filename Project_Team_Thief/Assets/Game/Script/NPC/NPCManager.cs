using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField]
    private NPCController[] npcs;
    [SerializeField]
    private GameObject interActiveIcon;
    private float nearestNpcDist;
    private int nearestNpcIndex;
    private bool bNearestNpcExist = false;

    private Unit controlUnit;

    private void Start()
    {
        GameLoader.instance.AddSceneLoadCallback(FindNPC);
    }

    public bool FindNPC(ref string ErrorMessage)
    {
        npcs = GameObject.FindObjectsOfType<NPCController>();

        foreach (var npc in npcs)
        {
            var go = Instantiate(interActiveIcon, npc.interactorableNoticeIcon);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
        }

        nearestNpcIndex = 0;
        return true;
    }

    private void Update()
    {
        controlUnit = GameManager.instance.ControlActor.GetUnit();
        nearestNpcDist = float.MaxValue;

        if (controlUnit == null || npcs == null)
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
            }
        }

        if(nearestNpcDist <= 9)
        {
            npcs[nearestNpcIndex].ActiveIcon(true);
            bNearestNpcExist = true;
        }
    }

    public NPCController GetNearestNPC()
    {
        if (bNearestNpcExist)
            return npcs[nearestNpcIndex];
        else
            return null;
    }

    public void InterAct()
    {
        if (!(bNearestNpcExist && GetNearestNPC().Act()))
        {
            PushEventQueue();
        }
    }

    public void PushEventQueue()
    {
        if (bNearestNpcExist)
        {
            GameManager.instance.GameEventSys.AddQueue(GetNearestNPC().npcName);
        }
    }
}
