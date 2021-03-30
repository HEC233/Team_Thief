using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Shadow
{
    public class ParticlePool
    {
        public GameObject shadowParticle;
        private ShadowParticle[] _pool;
        private bool _ready = false;

        private ShadowParticle head;

        public void Creat(int poolSize, VectorField vectorField, Transform parent)
        {
            _pool = new ShadowParticle[poolSize];
            for(int iter = 0; iter < poolSize; iter++)
            {
                _pool[iter] = GameObject.Instantiate(shadowParticle, Vector3.zero, Quaternion.identity, parent).GetComponent<ShadowParticle>();
                _pool[iter].SetVectorField(vectorField);
            }

            _ready = true;
        }

        //public void 
    }
}