using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

public class ShadowWalkColCtrl : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D _areaCollider2D;
    [SerializeField]
    private LayerMask _shadowLayerMask;
    private List<Collider2D> result;

    private Shadow _inAreaShadow;
    private ContactFilter2D _contactFilter2D;

    // Start is called before the first frame update
    void Start()
    {
        if (_areaCollider2D == null)
            Assert.IsNotNull("_basicAttackCollider Null");

        Init();
    }

    private void Init()
    {
        _inAreaShadow = null;
        result = new List<Collider2D>();

        _contactFilter2D.useTriggers = true;
        _contactFilter2D.useLayerMask = true;
        _contactFilter2D.layerMask = _shadowLayerMask;
    }

    public Shadow CheckAreaInsideShadow()
    {
        Debug.Log("asdasd");
        if(_areaCollider2D.IsTouchingLayers(_shadowLayerMask))
        {
            _areaCollider2D.OverlapCollider(_contactFilter2D, result);
            float dist = 0.0f;
            foreach (var item in result)
            {
                if (item.tag.Contains("Shadow") == false)
                    continue;
                
                if(dist < Mathf.Abs(Vector2.Distance(transform.position, result[0].transform.position)))
                {
                    dist = Mathf.Abs(Vector2.Distance(transform.position, result[0].transform.position));
                    _inAreaShadow = item.GetComponent<Shadow>();
                }
            }

            return _inAreaShadow;
        }

        return null;
    }
}
