using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> unit = new List<GameObject>();

    public bool Spawn(string unitName, Vector3 position, Quaternion quaternion, int count)
    {
        //for(int i = 0; i < unit.Count; i++)
        //{
        //    var unitComponent = unit[i].GetComponentInChildren<Unit>();
        //    if(unitComponent?.GetUnitName() == unitName)
        //    {
        //        for (int j = 0; j < count; j++)
        //        {
        //            StartCoroutine(SpawnCoroutine(unit[i], position, quaternion));
        //        }
        //        return true;
        //    }    
        //}
        //return false;

        var unit = Addressable.instance.GetUnit(unitName);
        if(unit != null)
        {
            StartCoroutine(SpawnCoroutine(unit, position, quaternion));
            return true;
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

    private IEnumerator SpawnCoroutine(GameObject go, Vector3 position, Quaternion quaternion)
    {
        var fx = GameManager.instance?.FX;
        if (fx)
        {
            var controller = fx.Play("Spawn", position);
            //while(controller != null && !controller.isStopped)
            //{
            //    yield return null;
            //}
            yield return new WaitForSeconds(0.2f);
        }

        Instantiate(go, position, quaternion);

        yield break;
    }
}
