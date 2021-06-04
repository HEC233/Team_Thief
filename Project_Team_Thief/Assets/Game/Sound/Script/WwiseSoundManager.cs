using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSoundManager : MonoBehaviour
{
    public static WwiseSoundManager instance; 
    
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
        AkSoundEngine.Suspend();
    }

    public void ResumeAllSound()
    {
        AkSoundEngine.WakeupFromSuspend();
    }
    
}
