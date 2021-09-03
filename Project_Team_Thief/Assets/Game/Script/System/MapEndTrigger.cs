using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class MapEndTrigger : MonoBehaviour
{
    private UnityEvent _mapEndEvent;
    private bool invoked = false;

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
            Assert.IsNotNull(_mapEndEvent);
            _mapEndEvent.Invoke();
            invoked = true;
        }
    }
}
