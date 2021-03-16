using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

public class OneWayTile : MonoBehaviour
{
    private PlatformEffector2D platformEfc = null;

    private void Awake()
    {
        Init();
        // 첫 생성시 작동을 안하고 껐다 다시 켜야 작동을 한다. 이유는 모름
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void Init()
    {
        // 부모 오브젝트만 실행하기 위해서
        if (transform.parent.GetComponent<Tilemap>() != null)
            return;

        // assertion 부분
        Assert.IsNotNull(GetComponent<Tilemap>());
        Assert.IsNotNull(GetComponent<TilemapRenderer>());

        gameObject.AddComponent<TilemapCollider2D>();
        gameObject.AddComponent<PlatformEffector2D>();
        GetComponent<TilemapCollider2D>().usedByEffector = true;
        platformEfc = GetComponent<PlatformEffector2D>();

        // 플레이어만 통과시키기 위해 같은 오브젝트를 복사한다.
        GameObject go = Instantiate(this.gameObject, transform);
        PlatformEffector2D childPlatformEfc = go.GetComponent<PlatformEffector2D>();

        go.GetComponent<TilemapRenderer>().enabled = false;
        platformEfc.colliderMask = (int)0x00000100;
        childPlatformEfc.colliderMask = (int)0x7FFFFEFF;
        childPlatformEfc.surfaceArc = 360;
        FlipDirection(true);
    }

    public void FlipDirection(bool isUp)
    {
        if (isUp)
        {
            platformEfc.rotationalOffset = 0;
        }
        else
        {
            platformEfc.rotationalOffset = 180;
        }
    }
}
