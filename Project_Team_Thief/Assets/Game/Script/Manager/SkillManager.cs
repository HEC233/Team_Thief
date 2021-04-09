using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ISkillBase
{
    void InsertKey(char key);

    bool CheckCommand();
}

public abstract class SkillBase : ISkillBase
{
    private SOSkillData _skillData;

    public SOSkillData SkillData => _skillData;

    protected List<char> _commandList;
    
    public SkillBase(SOSkillData soSkillData)
    {
        _skillData = soSkillData;
    }

    public abstract void InsertKey(char key);

    public abstract bool CheckCommand();

    public abstract void ResetKey();
}

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private List<SOSkillData> _soSkillDatas;

    private Dictionary<string, SOSkillData> _skillDatas;
    private List<SkillCtrl> _skillCtrls;

    public event UnityAction<SOSkillData> OnSkillCastEvent;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _skillCtrls = new List<SkillCtrl>();
        _skillDatas = new Dictionary<string, SOSkillData>();
        
        SetSkillDic();
        CreateSkillCtrl();
    }

    private void SetSkillDic()
    {
        for (int i = 0; i < _soSkillDatas.Count; i++)
        {
            _skillDatas.Add(_soSkillDatas[i].skillName, _soSkillDatas[i]);
        }
    }

    private void CreateSkillCtrl()
    {
        for (int i = 0; i < _soSkillDatas.Count; i++)
        {
            _skillCtrls.Add(new SkillCtrl(_soSkillDatas[i]));
        }
    }
    
    public void Inputkey(char key)
    {
        for (int i = 0; i < _skillCtrls.Count; i++)
        {
            _skillCtrls[i].InsertKey(key);
        }

        if (key == 'Z' || key == 'X' || key == 'C' || key == 'S') // 커맨드 결정 키
        {
            for (int i = 0; i < _skillCtrls.Count; i++)
            {
                if (_skillCtrls[i].CheckCommand() == true)
                {
                    OnSkillCastEvent?.Invoke(_skillDatas[_skillCtrls[i].SkillData.skillName]);
                    Debug.Log("true");
                }
            }
        }
    }
    
   private class SkillCtrl : SkillBase
   {
       private readonly string _skillCommand;
       private int _commandCount;
       
       public SkillCtrl(SOSkillData soSkillData) : base(soSkillData)
       {
           _commandList = new List<char>();
           
           _skillCommand = SkillData.skillCommand;
       }
       
       public override void InsertKey(char key)
       {
           _commandList.Add(key);
       }

       public override bool CheckCommand()
       {
           if(_commandList.Count < _skillCommand.Length)
               return false;

           _commandList = _commandList.GetRange(_commandList.Count - _skillCommand.Length, _skillCommand.Length);
           
           for (int i = 0; i < _skillCommand.Length; i++)
           {
               if (_commandList[i] == _skillCommand[i])
                   _commandCount++;
           }

           if (_commandCount == _skillCommand.Length)
           {
               ResetKey();
               return true;
           }

           ResetKey();
                       
           return false;
       }

       public override void ResetKey()
       {
           _commandList.Clear();
           _commandCount = 0;
       }
   }
}
