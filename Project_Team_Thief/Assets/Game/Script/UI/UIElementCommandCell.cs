using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIElementCommandCell : MonoBehaviour
{
    public enum CommandKey
    {
        up, down, right, left, z, x , c, space
    }

    public TextMeshProUGUI text;

    public void SetCommand(CommandKey key)
    {
        text.color = Color.yellow;
        switch (key)
        {
            case CommandKey.right:
                text.text = "RIGHT";
                text.fontSize = 5.5f;
                break;
            case CommandKey.left:
                text.text = "LEFT";
                text.fontSize = 6.5f;
                break;
            case CommandKey.up:
                text.text = "UP";
                text.fontSize = 7.5f;
                break;
            case CommandKey.down:
                text.text = "DOWN";
                text.fontSize = 6.5f;
                break;
            case CommandKey.x:
                text.text = "X";
                text.fontSize = 9f;
                break;
            case CommandKey.z:
                text.text = "Z";
                text.fontSize = 9f;
                break;
            case CommandKey.c:
                text.text = "C";
                text.fontSize = 9f;
                break;
            case CommandKey.space:
                text.text = "SPACE";
                text.fontSize = 5.5f;
                break;
        }
    }

    public void SetHighlight(bool value)
    {
        if (value)
        {
            text.color = Color.yellow;
        }
        else
        {
            text.color = Color.white;
        }
    }
}
