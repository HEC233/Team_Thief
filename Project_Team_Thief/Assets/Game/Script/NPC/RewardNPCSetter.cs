using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardNPCSetter : MonoBehaviour
{
    [SerializeField]
    private NPCReward[] _rewardNPCs = new NPCReward[2];
    [SerializeField]
    private NPCNextMap[] _nextMapPoints;

    private void Start()
    {
        _rewardNPCs[0].AddActListener(Hide);
        _rewardNPCs[1].AddActListener(Hide);
    }

    public void Show()
    {
        _rewardNPCs[0].gameObject.SetActive(true);
        _rewardNPCs[1].gameObject.SetActive(true);
    }
    public void Hide()
    {
        _rewardNPCs[0].gameObject.SetActive(false);
        _rewardNPCs[1].gameObject.SetActive(false);
    }

    public void Set(NPCRewardType[] cur, NPCRewardType[] next)
    {
        // 데이터 검사
        if(cur.Length < 2 || next.Length < 2)
        {
            return;
        }

        _rewardNPCs[0].SetRewardType(cur[0]);
        _rewardNPCs[1].SetRewardType(cur[1]);

        foreach (var nextMap in _nextMapPoints)
        {
            nextMap.SetRewardType(next);
        }
    }

    public NPCRewardType[] GetRandomType()
    {
        NPCRewardType[] types = new NPCRewardType[2];

        int first = Random.Range(1, 5);
        int second = Random.Range(1, 5);
        while(first == second)
        {
            second = Random.Range(0, 5);
        }
        types[0] = (NPCRewardType)first;
        types[1] = (NPCRewardType)second;

        return types;
    }
}
