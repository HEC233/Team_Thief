using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HpOrbGenerateor
{
    private static List<HpOrbController> orbs = new List<HpOrbController>();

    public static HpOrbController Generate(Vector3 pos)
    {
        HpOrbController target = null;
        foreach(var orb in orbs)
        {
            if(!orb.gameObject.activeSelf)
            {
                target = orb;
                break;
            }
        }
        if(target == null)
        {
            var go = GameObject.Instantiate(Addressable.instance.GetPrefab("HpOrb"), GameManager.instance.transform);
            orbs.Add(target = go.GetComponent<HpOrbController>());
        }

        target.Init();
        target.gameObject.transform.position = pos;

        return target;
    }
}
