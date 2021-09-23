using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICharacteristic : MonoBehaviour
{
    [SerializeField] 
    private List<SOCharacteristicBase> _characteristicDataBases;

    private List<SOCharacteristicBase> _inGameCharacteristicData;
    
    
    [SerializeField] 
    private UICharacteristicInfo[] _uiCharacteristicInfos;

    [SerializeField]
    private GameObject _UICharacteristic;

    private List<SOCharacteristicBase> _drawCharacteristicData = new List<SOCharacteristicBase>();
    
    private void Start()
    {
        ShowUICharacteristic(false);

        for (int i = 0; i < _characteristicDataBases.Count; i++)
        {
            _characteristicDataBases[i].RoadSpriteImage();
        }

        _inGameCharacteristicData = _characteristicDataBases.ToList();
    }
    
    public void ShowUICharacteristic(bool isActive)
    {
        _UICharacteristic.SetActive(isActive);

        if (isActive == true)
        {
            SetCharacteristicContent();
        }
    }

    public void SetCharacteristicContent()
    {
        DrawCharacteristic();
        
        for (int i = 0; i < _uiCharacteristicInfos.Length; i++)
        {
            _uiCharacteristicInfos[i].SetInfo(_drawCharacteristicData[i].SpriteImage);
        }
    }

    private void DrawCharacteristic()
    {
        _drawCharacteristicData.Clear();

        int index = 0;
        bool isAllDraw = true;

        while (isAllDraw)
        {
            for (int i = 0; i < _characteristicDataBases.Count; i++)
            {
                int probability = UnityEngine.Random.Range(0, 100);

                if (_characteristicDataBases[i].Probability < probability)
                {
                    _drawCharacteristicData.Add(_characteristicDataBases[i]);
                    index++;
                }

                if (index >= 3)
                {
                    isAllDraw = false;
                    break;
                }
            }
        }
    }

    public void RerollCharacteristicData()
    {
        
    }
    
}
