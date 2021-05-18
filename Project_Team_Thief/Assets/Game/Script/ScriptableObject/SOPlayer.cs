using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObject/Player")]
public class SOPlayer : ScriptableObject
{
    ///////////////////////////// 데이터로 관리 할 변수
    // 기본 스탯
    [SerializeField]
    private float _maxHp;

    public float MaxHP
    {
        get { return _maxHp; }
        set { _maxHp = value; }
    }

    [SerializeField]
    private float _curHp;

    public float CurHP
    {
        get { return _curHp; }
        set { _curHp = value; hpChangeEvent?.Invoke(); }
    }

    [SerializeField]
    private float _maxEncroachment;
    public float MaxEncroachment
    {
        get { return _maxEncroachment; }
        set { _maxEncroachment = value; }
    }

    [SerializeField] 
    private float _curEncroachment;

    public float CurEncroachment
    {
        get => _curEncroachment;
        set { _maxEncroachment = value;  }
    }

    public UnityAction hpChangeEvent;
    public UnityAction encroachmentChangeEvent;

    [SerializeField]
    private float _decreaseHp;
    [SerializeField]
    private float _encroachment;
}
