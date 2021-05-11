using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIElementCommandCell : MonoBehaviour
{
    public enum CommandKey
    {
        up, down, right, left, z, x , c, space
    }
    public List<Sprite> sprites = new List<Sprite>();

    public TextMeshProUGUI text;
    public Image image;

    public void SetCommand(CommandKey key)
    {
        text.color = Color.yellow;
        image.sprite = sprites[(int)key];
        switch (key)
        {
            case CommandKey.right:
                text.text = "→";
                text.fontSize = 17f;
                break;
            case CommandKey.left:
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
}
