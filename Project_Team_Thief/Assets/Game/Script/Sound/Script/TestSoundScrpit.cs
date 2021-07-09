using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSoundScrpit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void PlaySound()
    {
        AkSoundEngine.PostEvent("PC_HIT_hammer", gameObject);
    }

    public void PlaySound2()
    {
        AkSoundEngine.PostEvent("PC_BA2", gameObject);
    }
}
