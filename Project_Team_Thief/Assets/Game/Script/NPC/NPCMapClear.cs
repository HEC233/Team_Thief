using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMapClear : NPCController
{
    [SerializeField]
    private MapEndTrigger _mapEndTrigger;
    [SerializeField]
    private RewardNPCSetter _rewardNpcSetter;
    private bool _mapCleared = false;

    private void Start()
    {
        GameManager.instance.AddMapEndEventListener(ClearMap);
        _sendQueue = false;
        _rewardNpcSetter.Hide();
    }

    public void ClearMap()
    {
        _mapCleared = true;
    }

    public override bool Act()
    {
        if(_mapCleared)
        {
            _rewardNpcSetter.Show();
            return true;
        }
        else
        {
            return false;
        }
    }
}
