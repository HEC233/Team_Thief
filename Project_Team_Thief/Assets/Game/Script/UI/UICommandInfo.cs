using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UICommandInfo : MonoBehaviour
{
    private RectTransform _rect;
    private List<CommandManager.CommandCtrl> _commandDatas;
    public Transform verticalPanel;
    public RectTransform verticalPanelRect;
    public Transform horizonalPanel;
    public GameObject commandAssist;
    public GameObject commandCoolTime;

    private Transform playerTr;

    private List<UIElementCommand> panelList = new List<UIElementCommand>();
    private List<UICommandCoolTime> coolTimeList = new List<UICommandCoolTime>();

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Init()
    {
        foreach(var p in panelList)
        {
            DestroyImmediate(p.gameObject);
        }
        panelList.Clear();
        foreach (var p in coolTimeList)
        {
            DestroyImmediate(p.gameObject);
        }
        coolTimeList.Clear();

        _commandDatas = GameManager.instance.commandManager.GetCommandCtrl();
        foreach (var c in _commandDatas)
        {
            string commandString = c.CommandString;
            UIElementCommand element = null;
            GameObject go = null;

            for (int i = 0; i < 2; i++)
            {
                go = GameObject.Instantiate(commandAssist, verticalPanel);
                go.transform.localScale = Vector3.one;
                element = go.GetComponent<UIElementCommand>();
                element.InitCommandInfo(c, commandString);
                panelList.Add(element);
                commandString = c.ReverseCommandString;
            }
            go = GameObject.Instantiate(commandCoolTime, horizonalPanel);
            go.transform.localScale = Vector3.one;
            var cool = go.GetComponent<UICommandCoolTime>();
            coolTimeList.Add(cool);
            element.SetCoolTimeComponent(cool);
        }

        playerTr = null;
    }

    private void Update()
    {
        if (playerTr == null)
        {
            playerTr = GameManager.instance.GetControlActor()?.GetUnit()?.transform;
        }
        else
        {
            verticalPanelRect.anchoredPosition = GameManager.instance.uiMng.GetScreenPos(playerTr.position + Vector3.down);

            foreach (var e in panelList)
            {
                e.gameObject.SetActive(e.GetValidCommandLength() != 0);
            }
        }
    }
}
