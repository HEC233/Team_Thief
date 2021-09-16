using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineSetDeadZone : MonoBehaviour
{
    [SerializeField] 
    private float ScreenY;
    [SerializeField] 
    private float DeadZoneHeight;
    [SerializeField]
    private CinemachineVirtualCamera _camera;
    
    public void SetDeadZone()
    {
        // var composer = GameManager.instance.CameraMng._mainVirtualCamera
        //     .GetCinemachineComponent<CinemachineFramingTransposer>();
        // composer.m_DeadZoneHeight = DeadZoneHeight;
        // composer.m_ScreenY = ScreenY;

        StartCoroutine(SetDeadZoneCoroutine());
    }

    IEnumerator SetDeadZoneCoroutine()
    {
        yield return null;
        
        var composer = GameManager.instance.CameraMng._mainVirtualCamera
            .GetCinemachineComponent<CinemachineFramingTransposer>();
        composer.m_DeadZoneHeight = DeadZoneHeight;

        yield return null;
        
        composer.m_ScreenY = ScreenY;
        
        composer.m_ScreenY = ScreenY - 0.2f;

    }
}
