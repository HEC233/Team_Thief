using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    private Transform _cameraTr;

    private Vector2 prevCameraPos;

    public ScrollLayer[] layers;

    // 이건 시간이 오래 걸릴것 같다. 로딩쪽에서 수행하도록 뺄까?
    private void Start()
    {
        foreach(var layer in layers)
        {
            layer.Create(transform);
        }

        _cameraTr = Camera.main.transform;
        prevCameraPos = (Vector2)_cameraTr.position;
    }

    private void Update()
    {
        Vector3 delta = (Vector2)_cameraTr.position - prevCameraPos;

        foreach (var layer in layers)
        {
            layer.Move(delta);
        }

        prevCameraPos = (Vector2)_cameraTr.position;
    }

    [System.Serializable]
    public class ScrollLayer
    {
        public string layerTag;
        public bool verticalScrolling;
        public float distance;

        private Transform _tr;
        private float _rcp;

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
            _rcp = 1 - (10 / distance);
        }

        public void Move(Vector3 delta)
        {
            var vrtDelta = new Vector3(delta.x, 0, 0);

            _tr.position += (verticalScrolling ? delta : vrtDelta) * _rcp;
        }
    }
        
}
