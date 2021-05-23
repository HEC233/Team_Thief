using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionAttackFx : MonoBehaviour
{
    public ParticleSystem particle;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    public Damage damage;

    private void OnParticleCollision(GameObject other)
    {
        WwiseSoundManager.instance?.PlayEventSound("Dandelion_Boom");

        int length = particle.GetCollisionEvents(other, collisionEvents);

        Unit unit = other.GetComponentInChildren<Unit>();
        if (unit == null)
            return;

        int i = 0;
        while (i < length)
        {
            if (unit.tag == "Player")
                unit.HandleHit(damage);
            i++;
        }
    }
}
