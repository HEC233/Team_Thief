using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.FX
{
    public class EffectController : MonoBehaviour
    {
        public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

        public bool isStopped
        {
            get
            {
                bool value = true;
                foreach (var ps in particleSystems)
                {
                    if (!ps.isStopped)
                    {
                        value = false;
                        break;
                    }
                }
                return value;
            }
        }

        public void Play()
        {
            foreach (var ps in particleSystems)
            {
                ps.Play();
            }
        }

        public void SetSpeed(float speed)
        {
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                main.simulationSpeed = speed;
            }
        }
    }
}