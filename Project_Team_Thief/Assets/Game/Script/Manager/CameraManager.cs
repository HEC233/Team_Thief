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

    private bool _isZoomIn = false;

    [SerializeField] 
    private CinemachineSetDeadZone _setDeadZone;
    
    public event UnityAction OnZoomInEndEvent = null;
    private Unit playerUnit;
    
    public Camera mainCam;

    private void Start()
    {
        Bind();
        GameManager.instance.AddMapStartEventListener(FindAndFollowPlayer);
    }

    public void Bind()
    {
        GameManager.instance.TimeMng.startHitstopEvent += OnHitStopEvnetCall;
        GameManager.instance.TimeMng.endHitstopEvent += OnHitStopEndEventCall;
    }

    private void UnBind()
    {
        GameManager.instance.TimeMng.startHitstopEvent -= OnHitStopEvnetCall;
        GameManager.instance.TimeMng.endHitstopEvent -= OnHitStopEndEventCall;
    }
    
    
    public void FindAndFollowPlayer()
    {
        FindCameras();
        _mainVirtualCamera.Follow = GameManager.instance.PlayerActor.GetUnit().transform;
        _zoomInVirtualCamera.Follow = GameManager.instance.PlayerActor.GetUnit().transform;
        _setDeadZone.SetDeadZone();
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

        mainCam = Camera.main;
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

        if (_isZoomIn == true)
        {
            return;
        }
        Debug.Log("ZoomIn");
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
        Debug.Log("ZOomOut");
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
        float timer = 0.04f;
        _isZoomIn = true;
        
        yield return new WaitForSeconds(0.04f);

        while (_cinemachineBrain.IsBlending)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForSeconds(GameManager.instance.TimeMng.FixedDeltaTime);
        }
        Debug.Log("코루틴End");
        OnZoomInEndEvent?.Invoke();
        _isZoomIn = false;
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
            _shakeTick += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _liveVirtualCameraMultiChannelPerlin.m_AmplitudeGain = 0;
        _liveVirtualCameraMultiChannelPerlin.m_FrequencyGain = 0;
    }
}
