using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public string name;
    private GameManager gm;
    private bool bInterActorable = false;
    public GameObject interactorableIcon;

    private void Start()
    {
        gm = GameManager.instance;
        interactorableIcon.SetActive(false);
    }

    private void Update()
    {
        if (gm.GetControlActor() == null)
            return;
        if (gm.GetControlActor().GetUnit() == null)
            return;
        if (gm.GetControlActor().GetUnit().tag != "Player")
            return;
        float dis = Vector3.Distance(gm.GetControlActor().GetUnit().transform.position, transform.position);
        if (dis <= 5 && dis <= gm.lengthOfNPC)
        {
            bInterActorable = true;
            interactorableIcon.SetActive(true);
            gm.lengthOfNPC = dis;
            gm.nearestNpcName = name;
        }
        else
        {
            bInterActorable = false;
            interactorableIcon.SetActive(false);
            if(gm.nearestNpcName == name)
            {
                gm.lengthOfNPC = float.MaxValue;
                gm.nearestNpcName = string.Empty;
            }
        }
    }
}
