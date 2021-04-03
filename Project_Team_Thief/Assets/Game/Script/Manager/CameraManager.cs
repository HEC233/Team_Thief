using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineBrain _cinemachineBrain;
    private CinemachineVirtualCamera _liveVirtualCamera = null;
    private CinemachineBasicMultiChannelPerlin _liveVirtualCameraMultiChannelPerlin = null;

    public void OnLiveChange()
    {
        _liveVirtualCamera = _cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        _liveVirtualCameraMultiChannelPerlin =
            _liveVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
    public void Shake(float amplitudeGain, float frequencyGain, float time)
    {
        _liveVirtualCameraMultiChannelPerlin.m_AmplitudeGain = amplitudeGain;
        _liveVirtualCameraMultiChannelPerlin.m_FrequencyGain = frequencyGain;
        StartCoroutine(ShakeCoroutine(time));
    }
    
    public void Shake(string unitName)
    {

    }

    public void Zoom()
    {

    }

    IEnumerator GetVirtualCamera()
    {
        yield return null;
        _liveVirtualCamera = _cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        _liveVirtualCameraMultiChannelPerlin = _liveVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    IEnumerator ShakeCoroutine(float time)
    {
        float _shakeTick = 0.0f;
        while (_shakeTick < time)
        {
            _shakeTick += GameManager.instance.timeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _liveVirtualCameraMultiChannelPerlin.m_AmplitudeGain = 0;
        _liveVirtualCameraMultiChannelPerlin.m_FrequencyGain = 0;
    }
}
