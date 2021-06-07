using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSoundManager : MonoBehaviour
{
    public static WwiseSoundManager instance;
    private uint _bgmInGameSoundId;
    private uint _bgmMainSoundId;
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
            AkSoundEngine.StopPlayingID(_bgmMainSoundId);
            _isPlayingMainBgm = false;
        }

        _isPlayingMainBgm = true;
        _bgmMainSoundId = AkSoundEngine.PostEvent("Main", gameObject);
    }
    
    public void StopMainBgm()
    {
        _isPlayingMainBgm = false;
        AkSoundEngine.StopPlayingID(_bgmMainSoundId);
    }

    public void PlayInGameBgm()
    {
        if (_isPlayingInGameBgm == true)
        {
            AkSoundEngine.StopPlayingID(_bgmInGameSoundId);
            _isPlayingInGameBgm = false;
        }

        _isPlayingInGameBgm = true;
        _bgmInGameSoundId = AkSoundEngine.PostEvent("InGame", gameObject);
    }

    public void StopInGameBgm()
    {
        _isPlayingMainBgm = false;
        AkSoundEngine.StopPlayingID(_bgmInGameSoundId);
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
