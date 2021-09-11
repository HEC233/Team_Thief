using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

public class OneWayTile : MonoBehaviour
{
    private PlatformEffector2D platformEfc = null;
    public LayerMask playerLayer;

    private void Awake()
    {
        Init();
        // 첫 생성시 작동을 안하고 껐다 다시 켜야 작동을 한다. 이유는 모름
        //gameObject.SetActive(false);
        //gameObject.SetActive(true);
    }

    private void Init()
    {
        // 부모 오브젝트만 실행하기 위해서
        if (transform.parent.GetComponent<Tilemap>() != null)
        {
            DestroyImmediate(this);
            return;
        }
        platformEfc = gameObject.AddComponent<PlatformEffector2D>();
        platformEfc.useOneWay = true;
        platformEfc.surfaceArc = 170.0f;
        /*
        gameObject.AddComponent<TilemapCollider2D>();
        gameObject.AddComponent<PlatformEffector2D>();
        GetComponent<TilemapCollider2D>().usedByEffector = true;
        platformEfc = GetComponent<PlatformEffector2D>();
        */

        // 플레이어만 통과시키기 위해 같은 오브젝트를 복사한다.
        // 이 오브젝트는 플레이어 외의 다른 오브젝트들과 상호작용하며 플레이어가 아래로 이동할때 다른 오브젝트들도 함께 떨어지지 않도록 한다.
        GameObject go = Instantiate(this.gameObject, transform);
        PlatformEffector2D childPlatformEfc = go.GetComponent<PlatformEffector2D>();

        DestroyImmediate(go.GetComponent<TilemapRenderer>());
        platformEfc.colliderMask = playerLayer;
        childPlatformEfc.colliderMask = (int)0x7FFFFFFF - playerLayer;
        childPlatformEfc.surfaceArc = 180;
        FlipDirection(true);

        StartCoroutine(Flip());
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

    IEnumerator Flip()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                FlipDirection(false);
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                FlipDirection(true);
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
