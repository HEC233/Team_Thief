using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public string name;
    private bool bInterActorable = false;
    public Transform interactorableNoticeIcon;

    private void Start()
    {
        interactorableNoticeIcon.gameObject.SetActive(false);
    }

    public void ActiveIcon(bool bValue)
    {
        interactorableNoticeIcon.gameObject.SetActive(bValue);
    }
}
