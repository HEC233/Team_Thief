using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColor : MonoBehaviour
{
    SpriteRenderer sprd;
    private void Start()
    {
        sprd = GetComponent<SpriteRenderer>();
    }

    public void Set(Color color)
    {
        sprd.color = color;
    }
}
