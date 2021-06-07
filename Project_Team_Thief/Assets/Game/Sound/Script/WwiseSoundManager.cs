using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSoundManager : MonoBehaviour
{
    public static WwiseSoundManager instance;
    private uint _bgmInGameSoundId;
    private uint _bgmMainSoundId;
    
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
        _bgmMainSoundId = AkSoundEngine.PostEvent("Main", gameObject);
    }
    
    public void StopMainBgm()
    {
        AkSoundEngine.StopPlayingID(_bgmMainSoundId);
    }

    public void PlayInGameBgm()
    {
        _bgmInGameSoundId = AkSoundEngine.PostEvent("InGame", gameObject);
    }

    public void StopInGameBgm()
    {
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
