using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICommandInfo : MonoBehaviour
{
    private RectTransform _rect;
    private List<SOCommandData> _commandDatas;
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _commandDatas = GameManager.instance.commandManager.GetCommandData();
    }

    private void Update()
    {
        foreach(var c in _commandDatas)
        {
        }
    }
}
