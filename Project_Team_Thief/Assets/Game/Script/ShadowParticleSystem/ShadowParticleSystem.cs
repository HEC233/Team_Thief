using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PS.Shadow
{
    public class ShadowParticleSystem : MonoBehaviour
    {
        ParticlePool particle = new ParticlePool();
        VectorField vectorField;

        public ShadowParticle sp;

        private void Start()
        {
            vectorField = new VectorField(Screen.width / 10, Screen.height / 10);

            sp.SetVectorField(vectorField);
            sp.Init(sp.transform.position, new Vector2(-0.1f,0.1f), 100);
            sp.SetUseDrag(true);

            for (int y = 0; y < Screen.height / 10; y++)
            {
                for (int x = 0; x < Screen.width / 10; x++)
                {
                    vectorField.GetVector(x, y).y = -0.1f;
                }
            }
        }
    }
}