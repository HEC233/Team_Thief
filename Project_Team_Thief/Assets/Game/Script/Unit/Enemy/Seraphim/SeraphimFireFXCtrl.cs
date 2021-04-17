using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeraphimFireFXCtrl : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public GameObject light;
    private float[] thisPos = new float[2];
    private float[] lightPos = new float[2];

    private void Awake()
    {
        thisPos[0] = transform.localPosition.x;
        thisPos[1] = -thisPos[0];
        lightPos[0] = light.transform.localPosition.x;
        lightPos[1] = -lightPos[0];

    }

    public void TurnOffObject()
    {
        this.gameObject.SetActive(false);
    }

    public void SetFlip(bool value)
    {
        spriteRenderer.flipX = value;
        transform.localPosition = new Vector3(thisPos[value ? 1 : 0], transform.localPosition.y, transform.localPosition.z);
        light.transform.localPosition = new Vector3(lightPos[value ? 1 : 0], light.transform.localPosition.y, light.transform.localPosition.z);
    }
}
