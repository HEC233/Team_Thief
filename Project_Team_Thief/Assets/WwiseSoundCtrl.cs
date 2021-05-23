using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSoundCtrl : MonoBehaviour
{
    
    public uint PlayEventSound(string eventName)
    {
        return AkSoundEngine.PostEvent(eventName, gameObject);
    }
    
    public void StopEventSoundFromId(uint soundId)
    {
        AkSoundEngine.StopPlayingID(soundId);
    }
}
