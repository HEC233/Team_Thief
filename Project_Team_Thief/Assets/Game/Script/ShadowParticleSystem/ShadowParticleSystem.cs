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
        [SerializeField, Tooltip("플레이어 움직임에 따른 벡터장의 크기변화 스케일")] private float fieldChangePower = 1;
        [SerializeField, Tooltip("벡터장의 한 셀의 픽셀 크기")] private Vector2Int fieldCellSize = new Vector2Int(10,10);
        // 이건 임시
        [SerializeField] private bool useGravity = true;

        private List<BoxCollider2D> boxObjectOnVectorField = new List<BoxCollider2D>();
        private List<CapsuleCollider2D> capsuleObjectOnVectorField = new List<CapsuleCollider2D>();
        private List<Vector3> prevBoxPos = new List<Vector3>();
        private List<Vector3> prevCapsulePos = new List<Vector3>();

        private void Start()
        {
            vectorField = new VectorField(Screen.width, Screen.height, fieldCellSize);
            particles.Creat(particleCount, particleObject, vectorField, transform);

            StartCoroutine(vectorField.FieldRecoveryCoroutine());

        }

        private void Update()
        {
            for (int i = 0; i < boxObjectOnVectorField.Count; i++)
            {
                Vector3 dif = boxObjectOnVectorField[i].bounds.center - prevBoxPos[i];

                if (dif.sqrMagnitude > 0.1f)
                {
                    Vector3 urv = boxObjectOnVectorField[i].size / 2 + boxObjectOnVectorField[i].offset;
                    Vector3 dlv = -boxObjectOnVectorField[i].size / 2 + boxObjectOnVectorField[i].offset;

                    var ursc = Camera.main.WorldToScreenPoint(boxObjectOnVectorField[i].bounds.center + urv);
                    var dlsc = Camera.main.WorldToScreenPoint(boxObjectOnVectorField[i].bounds.center + dlv);

                    dif *= fieldChangePower;
                    vectorField.SetField((int)dlsc.x, (int)dlsc.y, (int)ursc.x, (int)ursc.y, new VectorCell(dif.x, dif.y));

                    prevBoxPos[i] = boxObjectOnVectorField[i].bounds.center;
                }
            }
            for (int i = 0; i < capsuleObjectOnVectorField.Count; i++)
            {
                Vector3 dif = capsuleObjectOnVectorField[i].bounds.center - prevCapsulePos[i];

                if (dif.sqrMagnitude > 0.1f)
                {
                    Vector3 urv = capsuleObjectOnVectorField[i].size / 2 + capsuleObjectOnVectorField[i].offset;
                    Vector3 dlv = -capsuleObjectOnVectorField[i].size / 2 + capsuleObjectOnVectorField[i].offset;

                    var ursc = Camera.main.WorldToScreenPoint(capsuleObjectOnVectorField[i].bounds.center + urv);
                    var dlsc = Camera.main.WorldToScreenPoint(capsuleObjectOnVectorField[i].bounds.center + dlv);

                    dif *= fieldChangePower;
                    vectorField.SetField((int)dlsc.x, (int)dlsc.y, (int)ursc.x, (int)ursc.y, new VectorCell(dif.x, dif.y));

                    prevCapsulePos[i] = capsuleObjectOnVectorField[i].bounds.center;
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
                p.UseGravity = useGravity;
            }
        }

        public void RegistCollider(BoxCollider2D collider)
        {
            boxObjectOnVectorField.Add(collider);
            prevBoxPos.Add(collider.bounds.center);
        }
        public void RegistCollider(CapsuleCollider2D collider)
        {
            capsuleObjectOnVectorField.Add(collider);
            prevCapsulePos.Add(collider.bounds.center);
        }
        public void UnregistCollider(BoxCollider2D collider)
        {
            prevBoxPos.RemoveAt(boxObjectOnVectorField.IndexOf(collider));
            boxObjectOnVectorField.Remove(collider);
        }
        public void UnregistCollider(CapsuleCollider2D collider)
        {
            prevCapsulePos.RemoveAt(capsuleObjectOnVectorField.IndexOf(collider));
            capsuleObjectOnVectorField.Remove(collider);
        }
    }
}