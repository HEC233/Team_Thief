using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PS.Shadow
{
    public class ShadowParticleSystem : MonoBehaviour
    {
        ParticlePool particles = new ParticlePool();
        VectorField vectorField;
        public GameObject particleObject;
        [SerializeField] private int particleCount;

        private List<BoxCollider2D> objectOnVectorField = new List<BoxCollider2D>();
        private List<Vector3> prevPos = new List<Vector3>();

        private void Start()
        {
            vectorField = new VectorField(Screen.width / 10, Screen.height / 10);
            particles.Creat(particleCount, particleObject, vectorField, transform);

            StartCoroutine(vectorField.FieldRecoveryCoroutine());

        }

        private void Update()
        {
            for (int i = 0; i < objectOnVectorField.Count; i++)
            {
                Vector3 dif = objectOnVectorField[i].bounds.center - prevPos[i];

                if (dif.sqrMagnitude > 0.1f)
                {
                    Vector3 urv = objectOnVectorField[i].size / 2 + objectOnVectorField[i].offset;
                    Vector3 dlv = -objectOnVectorField[i].size / 2 + objectOnVectorField[i].offset;

                    var ursc = Camera.main.WorldToScreenPoint(objectOnVectorField[i].bounds.center + urv);
                    var dlsc = Camera.main.WorldToScreenPoint(objectOnVectorField[i].bounds.center + dlv);

                    vectorField.SetField((int)dlsc.x / 10, (int)dlsc.y / 10, (int)ursc.x / 10, (int)ursc.y / 10, new VectorCell(dif.x, dif.y));

                    prevPos[i] = objectOnVectorField[i].bounds.center;
                }
            }
        }

        public void Burst(Vector3 pos, int particleCount, int speed, float lifeTime, bool useDrag = true)
        {
            for (int i = 0; i < particleCount; i++)
            {
                var p = particles.GetParticle();
                if (p == null) break;

                float theta = Random.Range(0, 360);
                p.Init(pos, new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * speed * 0.02f * Random.Range(0.8f, 1.2f), (int)lifeTime * 50, particles);
                p.UseDrag = useDrag;
                p.UseGravity = true;
            }
        }

        public void RegistCollider(BoxCollider2D collider)
        {
            objectOnVectorField.Add(collider);
            prevPos.Add(collider.bounds.center);
        }
    }
}