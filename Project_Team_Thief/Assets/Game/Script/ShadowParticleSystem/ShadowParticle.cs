using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Shadow
{
    public class ShadowParticle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private VectorField _vectorField;
        private Vector2 _vel;
        private int _cycle;

        public ShadowParticle next;

        private bool _useGravity = false;
        private bool _useDrag = false;

        public VectorField VectorField
        {
            set { _vectorField = value; }
        }

        public bool UseGravity
        {
            set { _useGravity = value; }
        }
        public bool UseDrag
        {
            set { _useDrag = value; }
        }

        public void Init(Vector3 pos, Vector2 velocity, int lifeCycle, ParticlePool pool = null)
        {
            transform.position = pos;
            _vel = velocity;
            _cycle = lifeCycle;
            StartCoroutine(process(pool));
            next = null;
        }

        IEnumerator process(ParticlePool pool)
        {
            Color color = spriteRenderer.color;
            int fullcycle = _cycle;
            while (_cycle-- > 0)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

                VectorCell vc = _vectorField.GetVectorWithScreenPos(screenPos.x, screenPos.y);

                _vel += vc.vector * 0.02f;

                if (_useGravity)
                    transform.Translate(new Vector3(0, -0.1f, 0));
                if (_useDrag)
                    _vel = _vel * 0.9f;

                transform.Translate(_vel);

                spriteRenderer.color = new Color(color.r, color.g, color.b, (float)_cycle / fullcycle);

                yield return new WaitForSeconds(0.02f);
            }

            End(pool);
        }

        public void End(ParticlePool pool)
        {
            gameObject.SetActive(false);

            if (pool != null)
            {
                var temp = pool.head;
                pool.head = this;
                next = temp;
            }
        }
    }
}