//https://blog.naver.com/PostView.nhn?blogId=2983934&logNo=221426046098&proxyReferer=https:%2F%2Fwww.google.com%2F
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventListener : MonoBehaviour
{
    [SerializeField] 
    private UnityEvent[] _events;

    public void EventCall(int index)
    {
        if (index >= 00 && index < _events.Length)
        {
            _events[index].Invoke();
        }
        else
        {
            Debug.LogError("Index out of range");
        }
    }
}
