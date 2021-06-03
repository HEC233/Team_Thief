using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> unit = new List<GameObject>();

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
}
