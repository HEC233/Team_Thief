using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public string name;
    private bool bInterActorable = false;
    public GameObject interactorableNoticeIcon;

    private void Start()
    {
        interactorableNoticeIcon.SetActive(false);
    }

    public void ActiveIcon(bool bValue)
    {
        interactorableNoticeIcon.SetActive(bValue);
    }
}
