using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameSkillObject : MonoBehaviour
{
    [SerializeField]
    private SkillControllerBase _controller = null;

    public void InitSkill(SkillControllerBase controller)
    {
        _controller = controller;
        _controller.OnEndSkillEvent += EndSkillEvent;
        _controller.Invoke();
    }

    private void EndSkillEvent()
    {
        _controller.Release();
        Release();
    }

    private void OnDestroy()
    {
        _controller?.Release();
        Release();
    }

    public CinemachineImpulseSource AddCinemachineImpulseComponent()
    {
        this.gameObject.AddComponent<CinemachineImpulseSource>();
        
        return gameObject.GetComponent<CinemachineImpulseSource>();
    }
    
    public void DestroyComponent(UnityEngine.Object component)
    {
        Destroy(component);
    }
    
    public void Release()
    {
        _controller = null;
        GameManager.instance.GameSkillMng.EnqueueSkillObject(this);
    }
}
