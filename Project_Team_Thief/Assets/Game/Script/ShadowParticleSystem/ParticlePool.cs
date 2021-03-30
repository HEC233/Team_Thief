using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Shadow
{
    public class ParticlePool
    {
        private GameObject shadowParticle;
        private ShadowParticle[] _pool;
        private bool _ready = false;

        public ShadowParticle head = null;

        public void Creat(int poolSize, GameObject shadowParticle, VectorField vectorField, Transform parent)
        {
            _pool = new ShadowParticle[poolSize];

            for(int iter = 0; iter < poolSize; iter++)
            {
                _pool[iter] = GameObject.Instantiate(shadowParticle, Vector3.zero, Quaternion.identity, parent).GetComponent<ShadowParticle>();
                _pool[iter].VectorField = vectorField;
                _pool[iter].End(this);
            }

            _ready = true;
        }

        public ShadowParticle GetParticle()
        {
            if (!_ready)
                return null;
            if (head == null)
                return null;

            var returnValue = head;
            head = returnValue.next;
            returnValue.gameObject.SetActive(true);

            return returnValue;
        }
    }
}