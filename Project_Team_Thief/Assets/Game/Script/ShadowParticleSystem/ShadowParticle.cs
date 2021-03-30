using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Shadow
{
    public class ShadowParticle : MonoBehaviour
    {
        private VectorField _vectorField;
        private Vector2 _dir;
        private int _cycle;

        public ShadowParticle next;

        private bool _useDrag = false;
        public void SetVectorField(VectorField vectorField)
        {
            _vectorField = vectorField;
        }

        public void SetUseDrag(bool value)
        {
            _useDrag = value;
        }

        public void Init(Vector3 pos, Vector2 direction, int lifeCycle)
        {
            transform.position = pos;
            _dir = direction;
            _cycle = lifeCycle;
            StartCoroutine(process());
        }

        IEnumerator process()
        {
            while (_cycle-- > 0)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position) / 10;

                VectorCell vc = _vectorField.GetVector((int)screenPos.x, (int)screenPos.y);

                transform.Translate(vc.x, vc.y, 0);

                transform.Translate(_dir);
                if (_useDrag)
                    _dir = _dir * 0.8f;

                yield return new WaitForSeconds(0.033f);
            }

            End();
        }

        public void End()
        {
            gameObject.SetActive(false);


        }
    }
}