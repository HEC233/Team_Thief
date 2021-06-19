using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSoundManager : MonoBehaviour
{
    public static WwiseSoundManager instance;
    private uint _bgmInGameSoundId;
    private uint _bgmMainSoundId;
    private uint _ambSoundId;
    private bool _isPlayingInGameBgm;
    private bool _isPlayingMainBgm;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void Start()
    {
        PlayMainBgm();
    }

    public void PlayMainBgm()
    {
        if (_isPlayingMainBgm == true)
        {
            StopMainBgm();
        }

        _isPlayingMainBgm = true;
        _bgmMainSoundId = AkSoundEngine.PostEvent("Main", gameObject);
    }
    
    public void StopMainBgm()
    {
        _isPlayingMainBgm = false;
        AkSoundEngine.StopPlayingID(_bgmMainSoundId);
        AkSoundEngine.StopAll();
    }

    public void PlayInGameBgm()
    {
        if (_isPlayingInGameBgm == true)
        {
            StopInGameBgm();
        }

        _isPlayingInGameBgm = true;
        _bgmInGameSoundId = AkSoundEngine.PostEvent("InGame", gameObject);
    }

    public void PlayAMBSound(string SceneName)
    {
        switch (SceneName)
        {
            case "HHG":
                _ambSoundId = AkSoundEngine.PostEvent("Forest", gameObject);
                break;
            case "BossState":
                _ambSoundId = AkSoundEngine.PostEvent("Forest", gameObject);
                break;
        }
    }

    public void StopInGameBgm()
    {
        _isPlayingMainBgm = false;
        AkSoundEngine.StopPlayingID(_bgmInGameSoundId);
        AkSoundEngine.StopAll();
    }

    public uint PlayEventSound(string eventName)
    {
        return AkSoundEngine.PostEvent(eventName, gameObject);
    }

    public void StopEventSoundFromId(uint soundId)
    {
        AkSoundEngine.StopPlayingID(soundId);
    }

    public void PauseAllSound()
    {
        AkSoundEngine.PostEvent("Pause", gameObject);
    }

    public void ResumeAllSound()
    {
        AkSoundEngine.PostEvent("Resume", gameObject);
        //AkSoundEngine.WakeupFromSuspend();
    }
    
}
