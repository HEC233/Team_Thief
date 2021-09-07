using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "SOZoomInOutData", menuName = "ScriptableObject//SOZoomInOutData")]
public class SOZoomInOutDataBase : ScriptableObject
{
    [SerializeField] 
    protected AnimationCurve _zoomInCurve;
    [SerializeField] 
    protected float _zoomInTime;
    [SerializeField] 
    protected float _zoomInSize;
    [SerializeField] 
    protected AnimationCurve _zoomOutCurve;
    [SerializeField]
    protected float _zoomOutTime;
    
    protected CinemachineBlendDefinition _cinemachineBlendDefinition;

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
    
}
