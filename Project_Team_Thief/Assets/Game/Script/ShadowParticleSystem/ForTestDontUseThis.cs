using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Shadow;

public class ForTestDontUseThis : MonoBehaviour
{
    public GameObject player;
    public ShadowParticleSystem sp;

    public Vector3 startPos = Vector3.zero;
    public int burstParticleCount;
    public int particleSpeed;
    public int particleLifeTime;
    public bool useDrag;

    private void Start()
    {
        sp.RegistCollider(player.GetComponent<BoxCollider2D>());
    }

    public void Func()
    {
        sp.Burst(startPos, burstParticleCount, particleSpeed, particleLifeTime, useDrag);
    }
}
