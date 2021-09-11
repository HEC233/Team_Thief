using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFragmentGenerator
{
    private static List<LightFragmentController> frags = new List<LightFragmentController>();

    public static LightFragmentController Get(Vector3 pos, int amount)
    {
        if(frags.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                var frag = GameObject.Instantiate(Addressable.instance.GetPrefab("LightFragment(coin)"),
                        GameManager.instance.transform).GetComponentInChildren<LightFragmentController>();
                frag.ReturnToPool();
            }
        }

        var ret = frags[0];
        frags.Remove(ret);

        ret.Init(pos, amount);

        return ret;
    }

    public static void Return(LightFragmentController frag)
    {
        frags.Add(frag);
    }
}
