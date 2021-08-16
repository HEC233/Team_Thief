using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXClip
{
    BasicAttack = 0,
    BasicAttack2,
    BasicAttack3,
    BasicAttack4,
}

public class SoundManager : MonoBehaviour
{
    private int _curBGM;

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip[] _sfx;

    private void Start()
    {
        Bind();
    }

    private void Bind()
    {
        GameManager.instance.TimeMng.startHitstopEvent += StartHitStopEvent;
        GameManager.instance.TimeMng.endHitstopEvent += EndHitStopEvent;
        GameManager.instance.TimeMng.startBulletTimeEvent += StartBulletTimeEvent;
        GameManager.instance.TimeMng.endBulletTimeEvent += EndBulletTimeEvent;
    }

    private void UnBind()
    {
        GameManager.instance.TimeMng.startHitstopEvent -= StartHitStopEvent;
        GameManager.instance.TimeMng.endHitstopEvent -= EndHitStopEvent;
        GameManager.instance.TimeMng.startBulletTimeEvent -= StartBulletTimeEvent;
        GameManager.instance.TimeMng.endBulletTimeEvent -= EndBulletTimeEvent;
    }

    // 효과음 재생 soundId는 아직 어떤 형태일지 정해지지 않았습니다.
    public void PlaySFX(SFXClip sfxClip)
    {
        _audioSource.clip = _sfx[(int)sfxClip];
        _audioSource.Play();        
    }

    // bgm은 기본적으로 루프이기 때문에 다음과 같은 구조로 작성하였음.

    // 배경음악 재생
    public void PlayBGM()
    {

    }

    // 배경음악 끄기
    public void StopBGM()
    {

    }

    // 배경음악 바꾸기
    public void ChangeBGM(int soundId)
    {
        _curBGM = soundId;
    }

    private void StartHitStopEvent(float timeScale)
    {
        _audioSource.pitch = timeScale;
    }

    private void EndHitStopEvent(float timeScale)
    {
        _audioSource.pitch = timeScale;
    }

    private void StartBulletTimeEvent(float timeScale)
    {
        _audioSource.pitch = timeScale;
    }

    private void EndBulletTimeEvent(float timeScale)
    {
        _audioSource.pitch = timeScale;
    }
    
}
