using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncroachmentManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] 
    private List<BlessingPenaltyDataBase> _blessingPenaltyDatas;
    
    [Header("Encroachment")]
    private float _encroachment;
    public float Encroachment => _encroachment;

    private int _encroachmentNumber;
    public int EncroachmentNumber => _encroachmentNumber;

    private bool _isEncroachmentActiveDecreasedCoroutine;
    
    [SerializeField]
    private float _encroachmentDecreasedNumber;

    [SerializeField] 
    private float _encroachmentDecreasedPerTime;

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
        _encroachmentZeroTimer = 0.0f;
        _encroachmentNumber = 5;
        SetPenaltyDataContent();
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
        _isEncroachmentActiveDecreasedCoroutine = true;
        _isEndRoom = false;
    }
    
    // 방 전투가 끝나면 호출 되는 함수라 가정
    private void EndRoomSetting()
    {
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
    }

    private void EncroachmentRecovery()
    {
        _encroachmentRecoveryAmount = CalcEncroachmentRecoveryAmount();
        
        ChangeEncroachment(_encroachmentRecoveryAmount);
    }

    // 어떠한 공식으로 회복량을 정할 껀지 필요.
    private float CalcEncroachmentRecoveryAmount()
    {
        return _encroachmentRecoveryAmount;
    }
        
    private void ChangeEncroachment(float encroachmentIncrease)
    {
        _encroachment += encroachmentIncrease;
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
        StartCoroutine(EncroachmentZeroTimerCorotuine());
        _isEncroachmentActiveDecreasedCoroutine = false;
    }

    // 잠식력이 0이되어 패털티 이벤트가 나온다는 이벤트를 던저 줄 함수.
    private void EncroachmentPenalty()
    {
        
    }

    public void ActivePenaltyFromId(int penaltyId)
    {
        var penaltyData = _blessingPenaltyDatas.Find(x => x.ID == penaltyId);
        penaltyData.ActivePenalty(GameManager.instance.PlayerActor.GetUnit());

        _blessingPenaltyDatas.Remove(penaltyData);
    }

    public BlessingPenaltyDataBase GetRandomBlessingPenalty()
    {
        int randIndex = UnityEngine.Random.Range(0, _blessingPenaltyDatas.Count);

        return _blessingPenaltyDatas[randIndex];
    }
    
    
    private void SetEncroachmentProduction()
    {
        if (_encroachment >= 80)
        {
            _encroachmentLevelIndex = 3;
        }
        else if (_encroachment >= 60)
        {
            _encroachmentLevelIndex = 2;
        }
        else if (_encroachment >= 40)
        {
            _encroachmentLevelIndex = 1;
        }
        else if (_encroachment >= 20)
        {
            _encroachmentLevelIndex = 0;
        }
        else if (_encroachment <= 10)
        {
            _encroachmentLevelIndex = 99;
        }

        EncroachmentProduction();
    }
    
    private void EncroachmentProduction()
    {
        switch (_encroachmentLevelIndex)
        {
            case 99:
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

        for (int i = 0; i < _encroachmentLevelArr.Length; i++)
        {
            if (_encroachmentLevelArr[i] == true)
            {
                if (i != _encroachmentLevelIndex)
                    StartCoroutine(EncroachmentProductionParticleFadeOut(i));
            }
            
            _encroachmentLevelArr[i] = false;
        }

        if (_encroachmentLevelIndex != 99)
            _encroachmentLevelArr[_encroachmentLevelIndex] = true;
    }
    
    IEnumerator EncroachmentProductionParticleFadeOut(int particlefadeOutIndex)
    {
        float timer = _encroachmentProductionFadeOutTime;
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

    IEnumerator EncroachmentZeroTimerCorotuine()
    {
        _encroachmentZeroTimer = 0.0f;
        
        while (!_isEndRoom)
        {
            _encroachmentZeroTimer += GameManager.instance.TimeMng.FixedDeltaTime;
            
            yield return new WaitForFixedUpdate();
        }
    }
    
    // IEnumerator EncroachmentRecoveryCoroutine()
    // {
    //     float timer = 0.0f;
    //     while (true)
    //     {
    //         if (_isEncroachmentActiveDecreasedCoroutine == false)
    //         {
    //             timer = 0.0f;
    //             yield return new WaitForFixedUpdate();
    //         }
    //         
    //         timer += GameManager.instance.TimeMng.FixedDeltaTime;
    //
    //         if (timer >= _encroachmentDecreasedPerTime)
    //         {
    //             timer = 0.0f;
    //             ChangeEncroachment(_encroachmentRecoveryAmount);
    //         }
    //
    //         yield return new WaitForFixedUpdate();
    //     }
    // }
    
}
