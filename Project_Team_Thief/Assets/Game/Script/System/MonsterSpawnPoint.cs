using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterSpawnPoint : MonoBehaviour, IEndTriggeCheck
{
    [SerializeField]
    private SpawnData[] _spawnDatas;

    [SerializeField]
    private bool _startWhenBegin = true;
    private bool _spawned = false;

    private List<MonsterUnit> _spawnedMonsters = new List<MonsterUnit>();
    private int _allMonsterCount = 0;
    private int _spawnedMonsterCount;

    private int _curMonsterCount = 0;

    [SerializeField, Tooltip("몬스터가 모두 죽을 경우 발생하는 이벤트")]
    private UnityEvent _monsterAllDeathEvent;
    [SerializeField, Tooltip("몬스터가 모두 죽을 경우 이벤트 시스템에 넘겨주는 큐 (Empty일시 아무것도 전달되지 않음)")]
    private string _monsterAllDeathQueue;

    /// <summary>
    /// 몬스터 풀을 순환하기 때문에 너무 자주 호출하지 말것
    /// </summary>
    public int CurMonsterCount
    {
        get
        {
            return _curMonsterCount; 
        }
    }

    public bool IsAllSpawned
    {
        get{ return _spawnedMonsterCount == _allMonsterCount; }
    }

    private void Start()
    {
        if(_startWhenBegin)
        {
            SpawnMonster();
        }
        foreach(var data in _spawnDatas)
        {
            _allMonsterCount += data.count;
        }
    }

    private void FixedUpdate()
    {
        if(!IsAllSpawned)
        {
            return;
        }
        if(CurMonsterCount == 0)
        {
            _monsterAllDeathEvent.Invoke();
            if (_monsterAllDeathQueue != null)
            {
                GameObject.Find("GameEventSystem")?.GetComponent<GameEventSystem>().AddQueue(_monsterAllDeathQueue);
            }
            this.enabled = false;
        }
    }

    public void SpawnMonster()
    {
        if(_spawned)
        {
            return;
        }
        foreach(var data in _spawnDatas)
        {
            StartCoroutine(SpawnCoroutine(data));
        }
        _spawned = true;
    }

    IEnumerator SpawnCoroutine(SpawnData data)
    {
        yield return new WaitForSeconds(data.enterDelay);

        if (data.interval == 0)
        {
            var list = GameManager.instance.Spawner.SpawnManyMU(data.unitName, data.position.position, data.count);
            _spawnedMonsters.AddRange(list);
            _spawnedMonsterCount += data.count;
            _curMonsterCount += data.count;
            foreach(var m in list)
            {
                m.DestroyEvent.AddListener(MonsterDeathCounter);
            }
        }
        else
        {
            int left = data.count;
            while (left > 0)
            {
                var obj = GameManager.instance.Spawner.SpawnMU(data.unitName, data.position.position);
                _spawnedMonsters.Add(obj);
                _spawnedMonsterCount++;
                _curMonsterCount ++;
                obj.DestroyEvent.AddListener(MonsterDeathCounter);

                left--;
                yield return new WaitForSeconds(data.interval);
            }
        }
    }

    public void MonsterDeathCounter()
    {
        _curMonsterCount--;
    }

    bool IEndTriggeCheck.Check()
    {
        return CurMonsterCount == 0 && IsAllSpawned;
    }
}
