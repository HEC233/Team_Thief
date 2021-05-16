using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private Damage damage;
    //private Rigidbody2D _rigid;
    //private CompositeCollider2D _collider;
    //private List<Collider2D> result = new List<Collider2D>();
    //private ContactFilter2D c;

    //private void Awake()
    //{
    //    _rigid = GetComponent<Rigidbody2D>();
    //    _collider = GetComponent<CompositeCollider2D>();
    //    c.NoFilter();
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        var unit = collision.gameObject.GetComponentInChildren<Unit>();
        if (unit == null)
            return;
        if (((1 << collision.gameObject.layer) & _playerLayer.value) != 0)
            unit.HandleHit(damage);
        else
        {
            var mUnit = collision.gameObject.GetComponentInChildren<MonsterUnit>();
            mUnit?.dieEvent.Invoke();
        }
    }

    //private void Update()
    //{
    //    if (_collider.IsTouchingLayers(_playerLayer))
    //    {
    //        _collider.OverlapCollider(c, result);
    //        foreach (var c in result)
    //        {
    //            var u = c.GetComponentInParent<Unit>();
    //            if (u == null)
    //                continue;
    //            if (u.tag == "Player")
    //                u.HandleHit(damage);
    //        }
    //    }
    //}
}
