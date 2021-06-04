using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    public CinemachineBrain _cinemachineBrain;
    public CinemachineVirtualCamera _mainVirtualCamera = null;
    public CinemachineVirtualCamera _liveVirtualCamera = null;
    private CinemachineBasicMultiChannelPerlin _liveVirtualCameraMultiChannelPerlin = null;
    [SerializeField] 
    private CinemachineBlenderSettings _cinemachineBlenderSettings;
    private CinemachineBlendDefinition _cinemachineBlendDefinition;
    public CinemachineVirtualCamera _zoomInVirtualCamera = null;

    public event UnityAction OnZoomInEndEvent = null;

    private void Start()
    {
        Bind();
    }

    public void Bind()
    {
        GameManager.instance.timeMng.startHitstopEvent += OnHitStopEvnetCall;
        GameManager.instance.timeMng.endHitstopEvent += OnHitStopEndEventCall;
    }

    private void UnBind()
    {
        GameManager.instance.timeMng.startHitstopEvent -= OnHitStopEvnetCall;
        GameManager.instance.timeMng.endHitstopEvent -= OnHitStopEndEventCall;
    }

    public void FindCameras()
    {
        if (_mainVirtualCamera == null)
        {
            _mainVirtualCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        }

        if (_zoomInVirtualCamera == null)
        {
            _zoomInVirtualCamera = GameObject.Find("CM ZoomVcam").GetComponent<CinemachineVirtualCamera>();
        }
    }
    
    public void OnLiveChange()
    {
        _liveVirtualCamera = _cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        _liveVirtualCameraMultiChannelPerlin =
            _liveVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
    public void Shake(float amplitudeGain, float frequencyGain, float time)
    {
        FindCameras();
        _liveVirtualCameraMultiChannelPerlin.m_AmplitudeGain = amplitudeGain;
        _liveVirtualCameraMultiChannelPerlin.m_FrequencyGain = frequencyGain;
        //StartCoroutine(ShakeCoroutine(time));
    }

    public void AllCinemachineFollowChange(Transform transform)
    {
        if (_mainVirtualCamera == null || _zoomInVirtualCamera == null)
        {
            FindCameras();
        }
        
        _zoomInVirtualCamera.m_Follow = transform;
        _mainVirtualCamera.m_Follow = transform;
    }
    
    public void Shake(string unitName)
    {

    }

    public void ZoomIn(CinemachineBlendDefinition customBlend, float zoomInSize)
    {
        if (_mainVirtualCamera == null || _zoomInVirtualCamera == null)
        {
            FindCameras();
        }
        
        _cinemachineBlenderSettings.m_CustomBlends[0].m_Blend = customBlend;
        _zoomInVirtualCamera.m_Lens.OrthographicSize = zoomInSize;
        
        _mainVirtualCamera.m_Priority = 0;
        StartCoroutine(WaitZoomInCoroutine());
        
    }

    public void ZoomOut(CinemachineBlendDefinition customBlend)
    {
        if (_mainVirtualCamera == null || _zoomInVirtualCamera == null)
        {
            FindCameras();
        }

        _cinemachineBlenderSettings.m_CustomBlends[1].m_Blend = customBlend;
        
        _mainVirtualCamera.m_Priority = 2;
    }

    private void OnHitStopEvnetCall(float timeScale)
    {
        CinemachineCore.UniformDeltaTimeOverride = 0;
    }

    private void OnHitStopEndEventCall(float timeScale)
    {
        CinemachineCore.UniformDeltaTimeOverride = -1;
    }

    IEnumerator WaitZoomInCoroutine()
    {
        yield return new WaitForSeconds(0.04f);
        
        while (_cinemachineBrain.IsBlending)
        {
            Debug.Log(_cinemachineBrain.IsBlending);
            yield return new WaitForSeconds(GameManager.instance.timeMng.FixedDeltaTime);
        }
        
        OnZoomInEndEvent?.Invoke();
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
