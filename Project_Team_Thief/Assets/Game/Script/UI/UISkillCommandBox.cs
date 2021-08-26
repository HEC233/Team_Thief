using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillCommandBox : MonoBehaviour
{
    private RectTransform _rect;

    public GameObject commandCell;

    public RectTransform frame;

    private List<UISkillCommandCell> cells = new List<UISkillCommandCell>();

    private bool ready = false;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    // 김태성 수정 commandData -> SkillSlot
    public void InitCommandInfo(string commandString)
    {
        int length = commandString.Length;
        int correction = 0;
        for (int i=0;i< commandString.Length; i++)
        {
            UISkillCommandCell.CommandKey key;
            switch (commandString[i])
            {
                case 'R':
                case 'r':
                    key = UISkillCommandCell.CommandKey.rightLeft;
                    break;
                case 'L':
                case 'l':
                    key = UISkillCommandCell.CommandKey.leftRight;
                    break;
                case 'U':
                case 'u':
                    key = UISkillCommandCell.CommandKey.up;
                    break;
                case 'D':
                case 'd':
                    key = UISkillCommandCell.CommandKey.down;
                    break;
                case 'Z':
                case 'z':
                    key = UISkillCommandCell.CommandKey.z;
                    break;
                case 'X':
                case 'x':
                    key = UISkillCommandCell.CommandKey.x;
                    break;
                case 'C':
                case 'c':
                    key = UISkillCommandCell.CommandKey.c;
                    break;
                case 'S':
                case 's':
                    key = UISkillCommandCell.CommandKey.space;
                    break;
                default:
                    length--;
                    correction++;
                    continue;
            }
            var go = GameObject.Instantiate(commandCell, transform);
            go.transform.localScale = Vector3.one;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMax = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchoredPosition = new Vector2(16 * (i - correction), 0f);

            var cell = go.GetComponent<UISkillCommandCell>();
            cell.SetCommand(key);
            cell.SetHighlight(false);
            cells.Add(cell);
        }

        frame.sizeDelta = new Vector2(16 * length, 15f);
        _rect.sizeDelta = new Vector2(16 * length, 16.0f);

        ready = true;
    }

    public void CommandUpdate(int ActiveLength, int CommandLength)
    {
        if (!ready)
            return;

        int i = 0;
        for (; i < ActiveLength; i++)
        {
            cells[i].SetHighlight(true);
        }
        for (; i < CommandLength; i++)
        {
            cells[i].SetHighlight(false);
        }
    }
}
