using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpOrbController : MonoBehaviour
{
    public BoxCollider2D checkBox;
    public LayerMask hitBoxLayer;
    private ContactFilter2D contactFilter = new ContactFilter2D();

    public Animator animator;

    private List<Collider2D> result = new List<Collider2D>();
    public Damage damage;

    private bool bConsumed = false;
    private float time = 0;

    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _time;


    private void Start()
    {
        Init();
        
        contactFilter.useTriggers = true;
        contactFilter.useLayerMask = true;
        contactFilter.SetLayerMask(hitBoxLayer);
    }
    public void Init()
    {
        bConsumed = false;
        time = 0;

        gameObject.SetActive(true);
        StartCoroutine(MoveAnimation());
    }

    IEnumerator MoveAnimation()
    {
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), 0);
        direction = direction.normalized;

        float t = 0;
        float speed = _speed;

        while (t < _time)
        {
            var d = GameManager.instance.timeMng.DeltaTime;
            t += d;
            var dt = d / _time;

            transform.position += direction * d * speed;

            speed -= _speed * dt;

            yield return null;
        }
    }

    private void Update()
    {
        if (bConsumed)
        {
            if (time > 0.9f)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (checkBox.IsTouchingLayers(hitBoxLayer))
            {
                checkBox.OverlapCollider(contactFilter, result);
                foreach (var c in result)
                {
                    var u = c.GetComponentInParent<Unit>();

                    if (u == null || u == this)
                        continue;
                    if (u.tag == "Player")
                    {
                        u.HandleHpRecovery(damage);

                        bConsumed = true;
                        animator.Play("Consumed");
                        time = 0;
                    }
                }
            }

            if (time >= 5)
            {
                gameObject.SetActive(false);
            }

        }
        time += GameManager.instance.timeMng.DeltaTime;
    }
}
