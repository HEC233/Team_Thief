using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class MapEndTrigger : MonoBehaviour
{
    public UnityEvent _mapEndEvent;
    private bool invoked = false;
    private IEndTriggeCheck[] _prerequires;

    [Header("Prerequirements")]
    [SerializeField]
    private MonsterSpawnPoint[] _spawnPoints = new MonsterSpawnPoint[0];
    [SerializeField]
    private string _mapEndTriggerString;

    private void Start()
    {
        int count = 0;
        count += _spawnPoints.Length;
        _prerequires = new IEndTriggeCheck[count];
        int index = 0;
        for(int i =0; i < _spawnPoints.Length; i++, index++)
        {
            _prerequires[index] = _spawnPoints[i];
        }
    }

    private bool CheckAllTrigger()
    {
        int count = 0;
        foreach(var check in _prerequires)
        {
            if(check.Check())
            {
                count++;
            }
        }
        return count == _prerequires.Length;
    }

    public void RegistUnityEvent(UnityEvent unityEvent)
    {
        _mapEndEvent = unityEvent;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(invoked)
        {
            return;
        }    
        if(collision.gameObject.tag == "Player" && collision.gameObject.layer == 10)
        {
            if (!CheckAllTrigger())
                return;
            Assert.IsNotNull(_mapEndEvent);
            _mapEndEvent.Invoke();
            if (_mapEndTriggerString != null)
            {
                GameManager.instance.GameEventSys.AddQueue(_mapEndTriggerString);
            }
            invoked = true;
        }
    }
}
