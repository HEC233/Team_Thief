using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> unit = new List<GameObject>();

    public bool Spawn(string unitName, Vector3 position, Quaternion quaternion, int count)
    {
        for(int i = 0; i < unit.Count; i++)
        {
            var unitComponent = unit[i].GetComponentInChildren<Unit>();
            if(unitComponent?.GetUnitName() == unitName)
            {
                for (int j = 0; j < count; j++)
                {
                    Instantiate(unit[i], position, quaternion);
                }
                return true;
            }    
        }
        return false;
    }

    public bool Spawn(string unitName, Vector3 position)
    {
        return Spawn(unitName, position, Quaternion.identity, 1);
    }
    public bool SpawnMany(string unitName, Vector3 position, int count)
    {
        return Spawn(unitName, position, Quaternion.identity, count);
    }
}
