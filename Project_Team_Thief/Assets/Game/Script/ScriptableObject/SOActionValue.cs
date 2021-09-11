using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "SOActionValue", menuName = "ScriptableObject/SOActionValue")]
public class SOActionValue : ScriptableObject
{
    [SerializeField]
    private SOZoomInOutDataBase _zoomInOutData;
    public SOZoomInOutDataBase ZoomInOutData => _zoomInOutData;

    [SerializeField] 
    private SOZoomInOutDataBase _zoomOutInData;
    public SOZoomInOutDataBase ZoomOutInData => _zoomOutInData; 
    
    [SerializeField]
    private SignalSourceAsset _cinemachineSignalSource;
    public SignalSourceAsset CinemachineSignalSource => _cinemachineSignalSource;
    
    [SerializeField]
    private float _hitstopTime;
    public float HitstopTime => _hitstopTime;

    [SerializeField] 
    private float _bulletTimeAmount;
    public float BulletTimeAmount => _bulletTimeAmount;

    [SerializeField] 
    private float _bulletTimeScale;
    public float BulletTimeScale => _bulletTimeScale;

    [SerializeField] 
    private FlashCtrl _flashCtrl;
    public FlashCtrl FlashCtrl => _flashCtrl;

    [SerializeField] 
    private GameObject _flashGO;
    public GameObject FlashGO => _flashGO;
}
