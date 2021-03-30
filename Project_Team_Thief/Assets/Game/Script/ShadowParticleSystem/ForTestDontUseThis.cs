using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Shadow;

public class ForTestDontUseThis : MonoBehaviour
{
    public GameObject player;
    public ShadowParticleSystem sp;

    private void Start()
    {
        sp.RegistCollider(player.GetComponent<BoxCollider2D>());
    }

    public void Func()
    {
        sp.Burst(new Vector3(0, 3, 0), 50, 10, 2, false);
    }
}
