using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardNPCSetter : MonoBehaviour
{
    [SerializeField]
    private NPCReward[] _rewardNPCs = new NPCReward[2];
    [SerializeField]
    private NPCController _mapClearPoint;

    // 클리어 포인트에 다음 맵 보상을 보여주기 위한 부분
    // 아직 어떤식으로 될지 기획서도 없어서 임시로 구현함, 변경해야 함
    public GameObject[] gos = new GameObject[5];

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

        GameObject.Instantiate(gos[(int)next[0]], _mapClearPoint.transform.position + new Vector3(-1.5f, 1.5f), Quaternion.identity, _mapClearPoint.gameObject.transform);
        GameObject.Instantiate(gos[(int)next[1]], _mapClearPoint.transform.position + new Vector3(1.5f, 1.5f), Quaternion.identity, _mapClearPoint.gameObject.transform);
    }

    public NPCRewardType[] GetRandomType()
    {
        NPCRewardType[] types = new NPCRewardType[2];

        int first = Random.Range(0, 5);
        int second = Random.Range(0, 5);
        while(first == second)
        {
            second = Random.Range(0, 5);
        }
        types[0] = (NPCRewardType)first;
        types[1] = (NPCRewardType)second;

        return types;
    }
}
