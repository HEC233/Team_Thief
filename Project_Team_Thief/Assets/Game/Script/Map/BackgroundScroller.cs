using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    private Transform _closeBG;
    private Transform _middleBG;
    private Transform _farBG;

    public float closeDist;
    public float middleDist;
    public float farDist;
    private float closeRCP;
    private float middleRCP;
    private float farRCP;

    private Transform _cameraTr;

    private Vector2 prevCameraPos;

    // 이건 시간이 오래 걸릴것 같다. 로딩쪽에서 수행하도록 뺄까?
    private void Start()
    {

        GameObject closeGo = new GameObject("Close Backgrounds");
        GameObject middleGo = new GameObject("Middle Backgounds");
        GameObject farGo = new GameObject("Far Backgournds");

        _closeBG = closeGo.transform;
        _middleBG = middleGo.transform;
        _farBG = farGo.transform;
        _closeBG.parent = _middleBG.parent = _farBG.parent = transform;

        var close = GameObject.FindGameObjectsWithTag("CloseRange");
        var middle = GameObject.FindGameObjectsWithTag("MiddleRange");
        var far = GameObject.FindGameObjectsWithTag("FarRange");

        foreach (var go in close)
        {
            go.transform.SetParent(_closeBG);
        }
        foreach (var go in middle)
        {
            go.transform.SetParent(_middleBG);
        }
        foreach (var go in far)
        {
            go.transform.SetParent(_farBG);
        }

        closeRCP = 1 - (10 / closeDist);
        middleRCP = 1 - (10 / middleDist);
        farRCP = 1 - (10 / farDist);

        _cameraTr = Camera.main.transform;
        prevCameraPos = (Vector2)_cameraTr.position;
    }

    private void Update()
    {
        Vector3 delta = (Vector2)_cameraTr.position - prevCameraPos;
        delta = new Vector3(delta.x, 0, 0);

        _closeBG.position += delta * closeRCP;
        _middleBG.position += delta * middleRCP;
        _farBG.position += delta * farRCP;

        prevCameraPos = (Vector2)_cameraTr.position;
    }
}
