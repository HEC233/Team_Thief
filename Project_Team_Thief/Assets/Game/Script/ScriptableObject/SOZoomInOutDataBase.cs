using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "SOZoomInOutData", menuName = "ScriptableObject/SOZoomInOutData")]
public class SOZoomInOutDataBase : ScriptableObject
{
    [SerializeField] 
    private AnimationCurve _zoomInCurve;
    [SerializeField] 
    private float _zoomInTime;
    [SerializeField] 
    private float _zoomInSize;
    public float ZoomInSize => _zoomInSize;
    
    [SerializeField] 
    private AnimationCurve _zoomOutCurve;
    [SerializeField]
    private float _zoomOutTime;
    
    private CinemachineBlendDefinition _cinemachineBlendDefinition;

    public void ZoomIn()
    {
        _cinemachineBlendDefinition.m_Style = CinemachineBlendDefinition.Style.Custom;
        _cinemachineBlendDefinition.m_CustomCurve = _zoomInCurve;
        _cinemachineBlendDefinition.m_Time = _zoomInTime;

        GameManager.instance.CameraMng.ZoomIn(_cinemachineBlendDefinition, _zoomInSize);
    }
    
    private void ZoomOut()
    {
        _cinemachineBlendDefinition.m_Style = CinemachineBlendDefinition.Style.Custom;
        _cinemachineBlendDefinition.m_CustomCurve = _zoomOutCurve;
        _cinemachineBlendDefinition.m_Time = _zoomOutTime;
        
        GameManager.instance.CameraMng.ZoomOut(_cinemachineBlendDefinition);
    }

    public CinemachineBlendDefinition GetZoomInData()
    {
        _cinemachineBlendDefinition.m_Style = CinemachineBlendDefinition.Style.Custom;
        _cinemachineBlendDefinition.m_CustomCurve = _zoomInCurve;
        _cinemachineBlendDefinition.m_Time = _zoomInTime;

        return _cinemachineBlendDefinition;
    }

    public CinemachineBlendDefinition GetZoomOutData()
    {
        _cinemachineBlendDefinition.m_Style = CinemachineBlendDefinition.Style.Custom;
        _cinemachineBlendDefinition.m_CustomCurve = _zoomOutCurve;
        _cinemachineBlendDefinition.m_Time = _zoomOutTime;
        
        return _cinemachineBlendDefinition;
    }
    
}
