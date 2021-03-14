using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private int _curBGM;

    // 효과음 재생 soundId는 아직 어떤 형태일지 정해지지 않았습니다.
    public void PlaySFX(int soundId)
    {

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
}
