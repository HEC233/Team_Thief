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
    private Camera mainCam;

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
            bool result = false;
            UIElementCommand element = null;
            GameObject go = null;

            for (int i = 0; i < 2; i++)
            {
                go = GameObject.Instantiate(commandAssist, verticalPanel);
                go.transform.localScale = Vector3.one;
                element = go.GetComponent<UIElementCommand>();
                result = element.InitCommandInfo(c, commandString);
                Assert.IsTrue(result);
                panelList.Add(element);
                commandString = c.ReverseCommandString;

            }
            go = GameObject.Instantiate(commandCoolTime, horizonalPanel);
            go.transform.localScale = Vector3.one;
            var cool = go.GetComponent<UICommandCoolTime>();
            coolTimeList.Add(cool);
            element.SetCoolTimeComponent(cool);

            GameManager.instance.AddTextToDeveloperConsole(c.CommandData.skillName + " initializing " + (result ? "successed" : "failed"));
        }

        playerTr = null;
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (playerTr == null)
        {
            playerTr = GameManager.instance.GetControlActor()?.GetUnit()?.transform;
        }
        else
        {
            var screenPos = mainCam.WorldToScreenPoint(playerTr.position + Vector3.down);
            verticalPanelRect.anchoredPosition = new Vector2(screenPos.x / Screen.width * 480, screenPos.y / Screen.height * 270);

            foreach (var e in panelList)
            {
                e.gameObject.SetActive(e.GetValidCommandLength() != 0);
            }
        }
    }
}
