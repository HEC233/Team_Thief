using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILeftEnemyInfo : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rect;
    [SerializeField]
    private TextMeshProUGUI _leftEnemyText;
    [SerializeField]
    private GameObject _leftEnemyObject;
    [SerializeField]
    private GameObject _clearTextObject;
    private MonsterSpawnPoint[] _monsterSpawners;
    private int _monsterCount = 0;

    public void Init()
    {
        _monsterSpawners = GameObject.FindObjectsOfType<MonsterSpawnPoint>();
        _monsterCount = 0;
        ShowClearText(false);
    }

    private void FixedUpdate()
    {
        int newMonsterCount = GetMonsterCount();
        if(_monsterCount != newMonsterCount)
        {
            _monsterCount = newMonsterCount;
            UpdateText();
        }

        if(_monsterCount == 0 && MonsterAllSpawned())
        {
            ShowClearText(true);
        }
    }

    private void UpdateText()
    {
        _leftEnemyText.text = _monsterCount.ToString();
    }

    private void ShowClearText(bool value)
    {
        _leftEnemyObject.SetActive(!value);
        _clearTextObject.SetActive(value);
        if (value)
        {
            // start animationCoroutine here
        }
    }

    private int GetMonsterCount()
    {
        if (_monsterSpawners != null)
        {
            int count = 0;
            foreach (var spawner in _monsterSpawners)
            {
                count += spawner.CurRemainMonsterCount;
            }

            return count;
        }
        return 0;
    }

    private bool MonsterAllSpawned()
    {
        if(_monsterSpawners != null)
        {
            bool ret = true;

            foreach (var spawner in _monsterSpawners)
            {
                ret = ret & spawner.IsAllSpawned;
            }

            return ret;
        }

        return true;
    }
}
