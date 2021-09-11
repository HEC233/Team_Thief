using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField]
    protected string _npcName;
    public string NpcName => _npcName;
    public Transform interactorableNoticeIcon;
    [SerializeField]
    protected bool _sendQueue = true;
    public bool DoesSendQueue => _sendQueue;

    private void Start()
    {
        interactorableNoticeIcon.gameObject.SetActive(false);
    }

    public void ActiveIcon(bool bValue)
    {
        interactorableNoticeIcon.gameObject.SetActive(bValue);
    }

    public virtual bool Act()
    {
        return false;
    }
}
