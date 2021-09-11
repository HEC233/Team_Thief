using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFragmentController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    Transform _root;

    private bool _isActive = false;
    private float _timeCheck = 0.0f;
    [SerializeField]
    private float _existTime = 10.0f;
    private int _moneyAmount = 0;

    private void Update()
    {
        if(_isActive)
        {
            _timeCheck += GameManager.instance.TimeMng.DeltaTime;

            if(_timeCheck >= _existTime)
            {
                ReturnToPool();
            }
        }
    }

    IEnumerator MoveCoroutine()
    {
        _rigidbody.gravityScale = 0;
        Vector2 InitPos = _root.position;
        float deltaMove = 0;
        while (deltaMove <= 1f)
        {
            _rigidbody.MovePosition(InitPos + Vector2.up * deltaMove);
            deltaMove += GameManager.instance.TimeMng.DeltaTime * 1;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        _rigidbody.gravityScale = 1;
        var player = GameManager.instance.ControlActor.GetUnit();
        while ((player.transform.position - _root.position).sqrMagnitude > 16)
        {
            yield return null;
        }

        _rigidbody.gravityScale = 0;
        while (true)
        {
            _rigidbody.MovePosition(Vector2.Lerp(_root.position, player.transform.position, GameManager.instance.TimeMng.DeltaTime));
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isActive)
        {
            return;
        }
        if (collision.gameObject.tag == "Player" && collision.gameObject.layer == 10)
        {
            GameManager.instance.AddMoney(_moneyAmount);
            ReturnToPool();
        }
    }

    public void Init(Vector3 position, int amount)
    {
        _root.gameObject.SetActive(true);
        _moneyAmount = amount;
        _root.position = position;
        _isActive = true;
        _timeCheck = 0.0f;
        StartCoroutine(MoveCoroutine());
    }

    public void ReturnToPool()
    {
        _isActive = false;
        StopAllCoroutines();
        LightFragmentGenerator.Return(this);
        _root.gameObject.SetActive(false);
    }
}
