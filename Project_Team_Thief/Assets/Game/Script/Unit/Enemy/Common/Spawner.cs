using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> unit = new List<GameObject>();
    private int _IDcount = 1;

    public GameObject[] Spawn(string unitName, Vector3 position, Quaternion quaternion, int count)
    {
        GameObject[] ret = new GameObject[count];

        var unit = Addressable.instance.GetUnit(unitName);
        if(unit != null)
        {
            GameManager.instance?.FX.Play("Spawn", position);

            for (int i = 0; i < count; i++)
            {
                ret[i] = Instantiate(unit, position, quaternion);
                ret[i].GetComponentInChildren<MonsterUnit>().MonsterID = _IDcount++;
            }

            return ret;
        }
        return null;
    }

    public GameObject Spawn(string unitName, Vector3 position)
    {
        return Spawn(unitName, position, Quaternion.identity, 1)[0];
    }
    public GameObject[] SpawnMany(string unitName, Vector3 position, int count)
    {
        return Spawn(unitName, position, Quaternion.identity, count);
    }
    public MonsterUnit SpawnMU(string unitName, Vector3 position)
    {
        return Spawn(unitName, position, Quaternion.identity, 1)[0].GetComponentInChildren<MonsterUnit>();
    }
    public MonsterUnit[] SpawnManyMU(string unitName, Vector3 position, int count)
    {
        var objs = Spawn(unitName, position, Quaternion.identity, count);
        MonsterUnit[] ret = new MonsterUnit[objs.Length];
        for(int i =0; i < objs.Length; i++)
        {
            ret[i] = objs[i].GetComponentInChildren<MonsterUnit>();
        }
        return ret;
    }
}
