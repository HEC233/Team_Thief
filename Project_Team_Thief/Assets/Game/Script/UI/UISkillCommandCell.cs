using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkillCommandCell : MonoBehaviour
{
    public enum CommandKey
    {
        up, down, right, left, z, x , c, rightLeft, leftRight, space
    }
    public List<Sprite> sprites = new List<Sprite>();

    public TextMeshProUGUI text;
    public Image image;
    //public RectTransform effect;
    //public Image effectImage;

    public void SetCommand(CommandKey key)
    {
        text.color = Color.yellow;
        image.sprite = sprites[(int)key];
        //effectImage.sprite = image.sprite;
        switch (key)
        {
            case CommandKey.rightLeft:
                text.text = "→";
                text.fontSize = 17f;
                break;
            case CommandKey.leftRight:
                text.text = "←";
                text.fontSize = 17f;
                break;
            case CommandKey.up:
                text.text = "↑";
                text.fontSize = 17f;
                break;
            case CommandKey.down:
                text.text = "↓";
                text.fontSize = 17f;
                break;
            case CommandKey.x:
                text.text = "X";
                text.fontSize = 17f;
                break;
            case CommandKey.z:
                text.text = "Z";
                text.fontSize = 17f;
                break;
            case CommandKey.c:
                text.text = "C";
                text.fontSize = 17f;
                break;
            case CommandKey.space:
                text.text = "SPACE";
                text.fontSize = 13.5f;
                break;
        }

        if (key == CommandKey.space)
        {
            text.enabled = true;
            image.enabled = false;
        }
        else
        {
            text.enabled = false;
            image.enabled = true;
        }
    }

    public void SetHighlight(bool value)
    {
        if (value)
        {
            text.color = Color.yellow;
            image.color = Color.yellow;
        }
        else
        {
            text.color = Color.white;
            image.color = Color.white;
        }
    }

    /*
     * 안쓰지만 나중에 쓸지도
    private IEnumerator SkillCellAnimCrt()
    {
        effectImage.color = Color.yellow;
        float size = 20.0f;
        effect.sizeDelta = Vector2.one * size;

        while (size > 15.0f)
        {
            size -= GameManager.instance.TimeMng.DeltaTime;
        }

        effect.sizeDelta = Vector2.one * 15;

        yield return new WaitForFixedUpdate();
    }
    */
}
