using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementCommand : MonoBehaviour
{
    private RectTransform _rect;

    public Image coolTime;
    public GameObject commandCell;

    public RectTransform frame;
    private CommandManager.CommandCtrl _data;

    private List<UIElementCommandCell> cells = new List<UIElementCommandCell>();

    private bool ready = false;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public bool InitCommandInfo(CommandManager.CommandCtrl data)
    {
        _data = data;

        string command = _data.CommandString;

        for(int i=0;i<command.Length; i++)
        {
            UIElementCommandCell.CommandKey key;
            switch (command[i])
            {
                case 'R':
                case 'r':
                    key = UIElementCommandCell.CommandKey.right;
                    break;
                case 'L':
                case 'l':
                    key = UIElementCommandCell.CommandKey.left;
                    break;
                case 'U':
                case 'u':
                    key = UIElementCommandCell.CommandKey.up;
                    break;
                case 'D':
                case 'd':
                    key = UIElementCommandCell.CommandKey.down;
                    break;
                case 'Z':
                case 'z':
                    key = UIElementCommandCell.CommandKey.z;
                    break;
                case 'X':
                case 'x':
                    key = UIElementCommandCell.CommandKey.x;
                    break;
                case 'C':
                case 'c':
                    key = UIElementCommandCell.CommandKey.c;
                    break;
                case 'S':
                case 's':
                    key = UIElementCommandCell.CommandKey.space;
                    break;
                default:
                    return false;
            }
            var go = GameObject.Instantiate(commandCell, transform);
            go.transform.localScale = Vector3.one;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMax = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchoredPosition = new Vector2(16 * (i) + 16, 0f);

            var cell = go.GetComponent<UIElementCommandCell>();
            cell.SetCommand(key);
            cell.SetHighlight(false);
            cells.Add(cell);
        }

        frame.sizeDelta = new Vector2(16 + 16 * command.Length, 15f);
        _rect.sizeDelta = new Vector2(16 + 16 * command.Length, 16.0f);

        ready = true;
        return true;
    }

    private void Update()
    {
        if (!ready)
            return;

        var count = Mathf.Min(_data.CommandList.Count, cells.Count);
        int length = GetValidCommandLength();

        for (int i = 0; i < length; i++)
        {
            cells[i].SetHighlight(true);
        }
        for (int i = length; i < cells.Count; i++)
        {
            cells[i].SetHighlight(false);
        }
    }

    public int GetValidCommandLength()
    {
        int length;
        for (length = 0; length < _data.CommandList.Count; length++)
        {
            if (_data.CommandList[length] != _data.CommandString[length])
            {
                length = 0;
                break;
            }
        }
        return length;
    }
}
