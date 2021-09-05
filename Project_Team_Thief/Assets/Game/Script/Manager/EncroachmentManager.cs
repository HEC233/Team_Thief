using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncroachmentManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] 
    private List<BlessingPenaltyDataBase> _blessingPenaltyDatas = new List<BlessingPenaltyDataBase>();
    private List<BlessingPenaltyDataBase> _blessingPenaltyDataInGame = new List<BlessingPenaltyDataBase>();
    private List<BlessingPenaltyDataBase> _randBlessingPenaltyDataBases = new List<BlessingPenaltyDataBase>();

    
    [Header("Encroachment")]
    [SerializeField]
    private float _encroachment;
    public float Encroachment => _encroachment;

    private int _encroachmentNumber;
    public int EncroachmentNumber => _encroachmentNumber;

    private bool _isEncroachmentActiveDecreasedCoroutine;
    
    [SerializeField]
    private float _encroachmentDecreasedNumber;

    [SerializeField] 
    private float _encroachmentDecreasedPerTime;

    private Coroutine _encroachmentCoroutine;

    private float _encroachmentRecoveryAmount;  // 해당 수치는 감소된 수치와 반비례해서 증가됨.
    
    [SerializeField]
    private float _encroachmentProductionFadeOutTime;
    [SerializeField]
    private ParticleSystem[] _encroachmentProductionParticleSystems;
    
    private bool[] _encroachmentLevelArr = new bool[5] {false, false, false, false, false};
    private int _encroachmentLevelIndex = 0;
    private bool _nonEncroachment = true;

    private float _encroachmentZeroTimer;
    public float EncroachmentZeroTimer => _encroachmentZeroTimer;

    private bool _isEndRoom;
    
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _encroachment = 100;
        _encroachmentZeroTimer = 0.0f;
        _encroachmentNumber = 5;

        _blessingPenaltyDataInGame = _blessingPenaltyDatas.ToList();
        
        GameManager.instance.AddMapStartEventListener(StartRoomSetting);
        GameManager.instance.AddMapEndEventListener(EndRoomSetting);
    }

    private void SetPenaltyDataContent()
    {
        for (int i = 0; i < _blessingPenaltyDatas.Count; i++)
        {
            _blessingPenaltyDatas[i].SetContentString();
        }
    }

    // 인 게임 씬으로 들어가면 호출 될 함수.
    public void SetInGame()
    {
        // 파티클 시스템들 찾아야함.
    }
    

    // 방 전투가 시작되면 호출되는 함수라 가정
    private void StartRoomSetting()
    {
        //튜토리얼 맵, 상점맵에서는 발동되면 안됨.
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            return;
        }
            
        _isEncroachmentActiveDecreasedCoroutine = true;
        _isEndRoom = false;
       _encroachmentCoroutine = StartCoroutine(EncroachmentDecreasedCoroutine());
    }
    
    // 방 전투가 끝나면 호출 되는 함수라 가정
    private void EndRoomSetting()
    {
        //튜토리얼 맵, 상점맵에서는 발동되면 안됨.
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            return;
        }
        
        _isEncroachmentActiveDecreasedCoroutine = false;
        
        if (_encroachment <= 0)
        {
            EncroachmentPenalty();
        }
        else
        {
            EncroachmentRecovery();
        }
        
        _isEndRoom = true;
        
        StopCoroutine(_encroachmentCoroutine);
    }

    private void EncroachmentRecovery()
    {
        _encroachmentRecoveryAmount = CalcEncroachmentRecoveryAmount();
        Debug.Log(_encroachmentRecoveryAmount);
        ChangeEncroachment(_encroachmentRecoveryAmount);
    }

    // 어떠한 공식으로 회복량을 정할 껀지 필요.
    private float CalcEncroachmentRecoveryAmount()
    {
        if (_encroachment <= 0)
        {
            _encroachmentRecoveryAmount = 100;
        }
        else
        {
            _encroachmentRecoveryAmount = 20;
        }
        
        return _encroachmentRecoveryAmount;
    }
        
    private void ChangeEncroachment(float encroachmentIncrease)
    {
        _encroachment += encroachmentIncrease;

        if (_encroachment > 100)
        {
            _encroachment = 100;
        }
        
        SetEncroachmentProduction();
        
        // 한 칸이 모두 다 감소되는 경우
        if (_encroachment < 0)
        {
            SetEncroachmentZero();
        }
        
        if (_encroachmentNumber <= 0)
        {
            _encroachmentNumber = 0;
            // Player Dead 이벤트 호출
        }
    }

    private void SetEncroachmentZero()
    {
        _encroachment = 0;
        _encroachmentNumber--;
        StartCoroutine(EncroachmentZeroTimerCoroutine());
        _isEncroachmentActiveDecreasedCoroutine = false;
    }

    private void SetPenaltyZero()
    {
        for (int i = 0; i < _blessingPenaltyDataInGame.Count; i++)
        {
            _blessingPenaltyDataInGame[i].SetAddPenalty(_encroachmentZeroTimer);
        }
    }

    // 잠식력이 0이되어 패털티 이벤트가 나온다는 이벤트를 던저 줄 함수.
    private void EncroachmentPenalty()
    {
        SetPenaltyZero();
        SetPenaltyDataContent();
        GameManager.instance.UIMng.ShowPenaltyScreen(GetRandomBlessingPenalty());
    }

    public void ActivePenaltyFromId(int penaltyId)
    {
        var penaltyData = _blessingPenaltyDataInGame.Find(x => x.ID == penaltyId);
        penaltyData.ActivePenalty(GameManager.instance.PlayerActor.GetUnit());

        _blessingPenaltyDataInGame.Remove(penaltyData);
        EncroachmentRecovery();
    }

    public BlessingPenaltyDataBase[] GetRandomBlessingPenalty()
    {
        _randBlessingPenaltyDataBases.Clear();

        while (_randBlessingPenaltyDataBases.Count <= 2)
        {
            int randIndex = UnityEngine.Random.Range(0, _blessingPenaltyDataInGame.Count);

            if (_randBlessingPenaltyDataBases.Contains(_blessingPenaltyDataInGame[randIndex]) == false)
            {
                _randBlessingPenaltyDataBases.Add(_blessingPenaltyDataInGame[randIndex]);
            }
        }

        return _randBlessingPenaltyDataBases.ToArray();
    }
    
    
    private void SetEncroachmentProduction()
    {
        if (_encroachment >= 80)
        {
            _encroachmentLevelIndex = 4;
        }
        else if (_encroachment >= 60)
        {
            _encroachmentLevelIndex = 0;
        }
        else if (_encroachment >= 40)
        {
            _encroachmentLevelIndex = 1;
        }
        else if (_encroachment >= 20)
        {
            _encroachmentLevelIndex = 2;
        }
        else
        {
            _encroachmentLevelIndex = 3;
        }

        EncroachmentProduction();
    }
    
    private void EncroachmentProduction()
    {
        Debug.Log(_encroachmentLevelIndex + "Production");
        switch (_encroachmentLevelIndex)
        {
            case 4:
                if (_nonEncroachment == true)
                {
                    _nonEncroachment = false;
                    StartCoroutine(EncroachmentProductionParticleFadeOut(0));
                }
                
                WwiseSoundManager.instance.PlayEventSound("Encroaching_End");
                break;
            case 3:
                if (_encroachmentLevelArr[_encroachmentLevelIndex] == false)
                {
                    StartCoroutine(EncroachmentProductionParticleFadeOut(_encroachmentLevelIndex - 1));

                    _encroachmentProductionParticleSystems[_encroachmentLevelIndex].gameObject.SetActive(true);

                    _encroachmentLevelArr[_encroachmentLevelIndex] = true;
                }
                break;
            case 2:
                if (_encroachmentLevelArr[_encroachmentLevelIndex] == false)
                {
                    StartCoroutine(EncroachmentProductionParticleFadeOut(_encroachmentLevelIndex - 1));

                    _encroachmentProductionParticleSystems[_encroachmentLevelIndex].gameObject.SetActive(true);
                    _encroachmentLevelArr[_encroachmentLevelIndex] = true;
                }
                break;
            case 1:
                if (_encroachmentLevelArr[_encroachmentLevelIndex] == false)
                {
                    StartCoroutine(EncroachmentProductionParticleFadeOut(_encroachmentLevelIndex - 1));
                    
                    _encroachmentProductionParticleSystems[_encroachmentLevelIndex].gameObject.SetActive(true);
                    _encroachmentLevelArr[_encroachmentLevelIndex] = true;
                }
                break;
            case 0:
                if (_encroachmentLevelArr[_encroachmentLevelIndex] == false)
                {
                    _nonEncroachment = true;
                    _encroachmentProductionParticleSystems[_encroachmentLevelIndex].gameObject.SetActive(true);
                    _encroachmentLevelArr[_encroachmentLevelIndex] = true;
                    
                    WwiseSoundManager.instance.PlayEventSound("Encroaching");
                }
                break;
        }
        //
        // for (int i = 0; i < _encroachmentLevelArr.Length; i++)
        // {
        //     if (_encroachmentLevelArr[i] == true)
        //     {
        //         if (i != _encroachmentLevelIndex)
        //             StartCoroutine(EncroachmentProductionParticleFadeOut(i));
        //     }
        //     
        //     _encroachmentLevelArr[i] = false;
        // }
        //
        // if (_encroachmentLevelIndex != 99)
        //     _encroachmentLevelArr[_encroachmentLevelIndex] = true;
    }
    
    IEnumerator EncroachmentProductionParticleFadeOut(int particlefadeOutIndex)
    {
        float timer = _encroachmentProductionFadeOutTime;
        Debug.Log(particlefadeOutIndex);
        var particleChildrens = _encroachmentProductionParticleSystems[particlefadeOutIndex]
            .GetComponentsInChildren<ParticleSystem>();

        ParticleSystemRenderer particleSystemRenderer;
        Material particleMaterial;
        float programerAlpha = 0.0f;
        
        while (timer > 0.0f)
        {
            timer -= GameManager.instance.TimeMng.FixedDeltaTime;
            programerAlpha = Mathf.Lerp(0, 1, timer);

            for (int i = 1; i < particleChildrens.Length; i++)
            {
                particleSystemRenderer = particleChildrens[i].GetComponent<ParticleSystemRenderer>();
                particleMaterial = particleSystemRenderer.material;
                particleMaterial.SetFloat("Vector1_1F53A638", programerAlpha);
            }


            yield return new WaitForFixedUpdate();
        }
        
        for (int i = 1; i < particleChildrens.Length; i++)
        {
            particleSystemRenderer = particleChildrens[i].GetComponent<ParticleSystemRenderer>();
            particleMaterial = particleSystemRenderer.material;
            particleMaterial.SetFloat("Vector1_1F53A638", 1.0f);
        }
        
        _encroachmentProductionParticleSystems[particlefadeOutIndex].gameObject.SetActive(false);
    }

    IEnumerator EncroachmentZeroTimerCoroutine()
    {
        _encroachmentZeroTimer = 0.0f;
        
        while (!_isEndRoom)
        {
            _encroachmentZeroTimer += GameManager.instance.TimeMng.FixedDeltaTime;
            Debug.Log(_encroachmentZeroTimer);
            yield return new WaitForFixedUpdate();
        }
    }
    
    IEnumerator EncroachmentDecreasedCoroutine()
    {
        float timer = 0.0f;
        while (true)
        {
            if (_isEncroachmentActiveDecreasedCoroutine == false)
            {
                timer = 0.0f;
                yield return new WaitForFixedUpdate();
            }
            
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            
            if (timer >= _encroachmentDecreasedPerTime)
            {
                timer = 0.0f;
                ChangeEncroachment(_encroachmentDecreasedNumber);
            }
    
            yield return new WaitForFixedUpdate();
        }
    }
    
}
