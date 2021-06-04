using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Transform cameraTr;

    private Vector2 prevCameraPos;

    public ScrollLayer[] layers;

    // 이건 시간이 오래 걸릴것 같다. 로딩쪽에서 수행하도록 뺄까?
    private void Start()
    {
        foreach(var layer in layers)
        {
            layer.Create(transform);
            //StartCoroutine(layer.ScrollLerp(this));
        }

        //_cameraTr = Camera.main.transform;
        prevCameraPos = (Vector2)cameraTr.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = (Vector2)cameraTr.position - prevCameraPos;

        foreach (var layer in layers)
        {
            layer.Move(delta);
        }

        prevCameraPos = (Vector2)cameraTr.position;
    }

    [System.Serializable]
    public class ScrollLayer
    {
        public string layerTag;
        public bool verticalScrolling;
        public float distance;

        private Transform _tr;
        private float _rcp;

        private Vector3 targetPos;

        public void Create(Transform parent)
        {
            GameObject go = new GameObject(layerTag);

            _tr = go.transform;
            _tr.SetParent(parent);

            var gos = GameObject.FindGameObjectsWithTag(layerTag);

            foreach (var o in gos)
            {
                o.transform.SetParent(_tr);
            }
            _rcp = (10 / distance);

            targetPos = _tr.position;
        }

        public void Move(Vector3 delta)
        {
            if (verticalScrolling)
            {
                _tr.localPosition -= delta * _rcp;
            }
            else
            {
                _tr.localPosition -= new Vector3(delta.x * _rcp, delta.y, 0);
            }
        }
    }
        
}
