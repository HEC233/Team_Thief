using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BossAlterActor : MonoBehaviour, IActor
{
    private BossAlterUnit unit;
    [SerializeField]
    private AnimationCtrl animCtrl;
    [SerializeField]
    private WwiseSoundCtrl wWsieSoundCtrl;

    private int wave = 1;
    [SerializeField]
    private List<SpawnData> wave1Data;
    [SerializeField]
    private List<SpawnData> wave2Data;
    [SerializeField]
    private List<SpawnData> wave3Data;

    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private bool bSpawning = false;
    Coroutine EnemyDeadCheck;

    private void Awake()
    {
        unit = GetComponentInParent<BossAlterUnit>();
        Assert.IsNotNull(unit);
        unit.hitEvent.AddListener(HitTransition);
        unit.dieEvent.AddListener(DieTransition);
    }

    private void Start()
    {
        unit.SetInvincibility(true);
    }

    private void Spawn()
    {
        List<SpawnData> spawnData = new List<SpawnData>();
        switch (wave)
        {
            case 1:
                spawnData = wave1Data;
                break;
            case 2:
                spawnData = wave2Data;
                break;
            case 3:
                spawnData = wave3Data;
                break;
        }
        foreach (var data in spawnData)
        {
            StartCoroutine(SpawnCoroutine(data));
        }
        if(spawnData.Count == 0)
        {
            SetBossAttackable();
        }
    }

    private IEnumerator SpawnCoroutine(SpawnData data)
    {
        int curCount = 0;
        float timeCheck = 0;

        while (curCount < data.count)
        {
            bSpawning = true;
            if (timeCheck >= data.enterDelay + data.interval * curCount)
            {
                spawnedMonsters.Add(GameManager.instance.spawner.Spawn(data.unitName, data.position.position));
                curCount++;
            }

            timeCheck += GameManager.instance.timeMng.DeltaTime;

            yield return null;
        }
        bSpawning = false;
        if (EnemyDeadCheck == null)
        {
            EnemyDeadCheck = StartCoroutine(EnemyDeadCheckCoroutine());
        }
    }

    private IEnumerator EnemyDeadCheckCoroutine()
    {
        while (bSpawning)
        {
            yield return null;
        }

        while(spawnedMonsters.Count != 0)
        {
            for(int i = spawnedMonsters.Count - 1; i >= 0; i--)
            {
                if(spawnedMonsters[i] == null)
                {
                    spawnedMonsters.RemoveAt(i);
                }
            }

            yield return null;
        }

        SetBossAttackable();
        EnemyDeadCheck = null;
    }

    private void SetBossAttackable()
    {
        if (wave == 3)
        {
            GameManager.instance.uiMng.SetBossHPColor(Color.red, Color.gray);
        }
        else
        {
            GameManager.instance.uiMng.SetBossHPColor(Color.blue, Color.red);
        }

        GameManager.instance.uiMng.InitBossHP(unit);
        GameManager.instance.uiMng.BossHPUpdate();
        unit.SetInvincibility(false);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        switch (condition)
        {
            case TransitionCondition.BossAwake:
                unit.SetInvincibility(true);
                Spawn();
                break;
            case TransitionCondition.Hit:
                GameManager.instance.uiMng.BossHPUpdate();
                break;
            case TransitionCondition.Die:
                GameObject.Find("GameEventSystem")?.GetComponent<GameEventSystem>()?.AddDeadBossQueue(unit.GetUnitName());
                GameManager.instance.uiMng.BossDie();
                break;
        }


        return false;
    }

    public void HitTransition()
    {
        Transition(TransitionCondition.Hit);
    }

    public void DieTransition()
    {
        wave++;

        if (3 < wave)
        {
            Transition(TransitionCondition.Die);
        }
        else
        {
            unit.ResetHP();
            GameManager.instance.uiMng.BossDie();
            unit.SetInvincibility(true);
            Spawn();
        }
    }
}


[System.Serializable]
public struct SpawnData
{
    public Transform position;
    public string unitName;
    public int count;
    public float enterDelay;
    public float interval;
}
