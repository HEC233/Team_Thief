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
    //SOSkillData 자체를 전달해주는 액션 X
    //soSkillDAta 오픈시켜서 Get UnityAction은 키 값이 되는 string, int만 전달해주고 데이터는
    // 거기서 skillManager를 통해 가져올 것인가
    
    // 현재 스킬 매니저와 스킬 컨트롤이 나누긴 나눴지만 종속적인 형태
    // 이것이 클래스 외부로 나누게 될 경우에는 어떻게 될 것인가?
    // 역순으로 검사.

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

    // 1. 커맨드 결정키는 마지막 단 하나
    // LR Z
    // 2. 반드시 모든 스킬 커맨드에는 결정키가 하나 포함된다
    // 방향키로만 이루어진 스킬은 X
    // 3. 중복되는 커맨드는 존재하지 않음
    // LRZ   LRZLX XX
    // 던파에서 커맨드를 저장 위위 아래아래 Z 스킬 위위 아래 z
    // 중간에 다른 키가 입력됐을 때 스킬이 발동되는 경우?
    
        if (key == 'Z' || key == 'X' || key == 'C' || key == 'S') // 커맨드 결정 키
        {
            for (int i = 0; i < _skillCtrls.Count; i++)
            {
                if (_skillCtrls[i].CheckCommand() == true)
                {
                    OnSkillCastEvent?.Invoke(_skillDatas[_skillCtrls[i].SkillData.skillName]);
                    ResetAllCommandList();
                    break;
                }
            }
        }
    }

    private void ResetAllCommandList()
    {
        for (int i = 0; i < _skillCtrls.Count; i++)
        {
            _skillCtrls[i].ResetKey();
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
