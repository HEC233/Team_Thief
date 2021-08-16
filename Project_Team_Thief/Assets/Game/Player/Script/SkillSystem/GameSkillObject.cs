using System.Collections;
using System.Collections.Generic;
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
