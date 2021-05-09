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
    public GameObject commandAssist;

    private Transform playerTr;
    private Camera mainCam;

    private List<UIElementCommand> panelList = new List<UIElementCommand>();

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        playerTr = GameManager.instance.GetControlActor().GetUnit().transform;
        mainCam = Camera.main;
    }

    public void Init()
    {
        foreach(var p in panelList)
        {
            DestroyImmediate(p.gameObject);
        }
        panelList.Clear();

        _commandDatas = GameManager.instance.commandManager.GetCommandCtrl();
        foreach (var c in _commandDatas)
        {
            var go = GameObject.Instantiate(commandAssist, verticalPanel);
            go.transform.localScale = Vector3.one;
            var element = go.GetComponent<UIElementCommand>();
            Assert.IsTrue(element.InitCommandInfo(c));
            panelList.Add(element);
        }
    }

    private void Update()
    {
        foreach (var e in panelList)
        {
            e.gameObject.SetActive(e.GetValidCommandLength() != 0);
        }

        var screenPos = mainCam.WorldToScreenPoint(playerTr.position + Vector3.down);
        verticalPanelRect.anchoredPosition = new Vector2(screenPos.x / Screen.width * 480, screenPos.y / Screen.height * 270);
    }
}
