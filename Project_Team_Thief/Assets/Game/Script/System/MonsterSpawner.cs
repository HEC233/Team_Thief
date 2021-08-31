using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private SpawnData[] _spawnDatas;

    [SerializeField]
    private bool _startWhenBegin = true;
    private bool _spawned = false;

    private List<GameObject> _spawnedMonsters = new List<GameObject>();
    private int _allMonsterCount = 0;
    private int _spawnedMonsterCount;

    /// <summary>
    /// 몬스터 풀을 순환하기 때문에 너무 자주 호출하지 말것
    /// </summary>
    public int CurMonsterCount
    {
        get
        {
            for (int i = 0; i < _spawnedMonsters.Count; i++)
            {
                if (_spawnedMonsters[i] == null)
                {
                    _spawnedMonsters.RemoveAt(i);
                    i--;
                }
            }
            return _spawnedMonsters.Count; 
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
            var list = GameManager.instance.Spawner.SpawnMany(data.unitName, data.position.position, data.count);
            _spawnedMonsters.AddRange(list);
            _spawnedMonsterCount += data.count;
        }
        else
        {
            int left = data.count;
            while (left > 0)
            {
                var obj = GameManager.instance.Spawner.Spawn(data.unitName, data.position.position);
                _spawnedMonsters.Add(obj);
                _spawnedMonsterCount++;

                left--;
                yield return new WaitForSeconds(data.interval);
            }
        }
    }
}
