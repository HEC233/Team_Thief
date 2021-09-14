using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이름이 너무 길어서 추가한 축약어
using AbSt = AbnormalState;
// 이상상태를 나타내는데 쓰이는 열거형, 비트 연산을 통해 여러 상태를 중첩가능하다.
public enum AbnormalState
{
    None        = 0x00000000,
    Spare1      = 0x00000001,
    Spare2      = 0x00000002,
    Spare3      = 0x00000004,
    Spare4      = 0x00000008,
    Spare5      = 0x00000010,
    Spare6      = 0x00000020,
    Spare7      = 0x00000040,
    Spare8      = 0x00000080,
}

[System.Serializable]
public struct Damage
{
    public float power;
    public Vector2 knockBack;
    public Vector2 knockBackTime;
    public int abnormal;
    [HideInInspector]
    public Vector3 hitPosition;
    public int additionalInfo;
    public float stiffness; // 경직 시간
}
