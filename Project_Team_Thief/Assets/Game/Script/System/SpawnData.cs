using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnData
{
    public Transform position;
    public string unitName;
    public int count;
    public float enterDelay;
    public float interval;
}