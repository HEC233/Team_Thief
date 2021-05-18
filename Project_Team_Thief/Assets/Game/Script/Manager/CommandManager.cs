using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandManager : MonoBehaviour
{
    [SerializeField]
    //private float _commandInputTime = 0.03f;
    private float _commandInputTime = 0.1f;

    private float _inputTime;
    private float _beInputTime;

    [SerializeField] private List<SOCommandData> _soCommandDatas;

    private List<CommandCtrl> _commandCtrls;
    //private Dictionary<string, SOCommandData> _commandDatas;

    //---
    public List<CommandCtrl> GetCommandCtrl()
    {
        return _commandCtrls;
    }
    //---

    public event UnityAction<string, bool> OnCommandCastEvent;
    // 커맨드가 입력됐을 때 데이터를 어떻게 전달 해야 할 까?
    // SkillDataLibrary Class를 하나 만들어서 거기서 받아오는건 어떨까?

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _commandCtrls = new List<CommandCtrl>();
        //_commandDatas = new Dictionary<string, SOCommandData>();

        SetCommandDic();
        CreateCommandCtrl();
    }

    private void SetCommandDic()
    {
        for (int i = 0; i < _soCommandDatas.Count; i++)
        {
            //_commandDatas.Add(_soCommandDatas[i].skillName, _soCommandDatas[i]);
        }
    }

    private void CreateCommandCtrl()
    {
        for (int i = 0; i < _soCommandDatas.Count; i++)
        {
            _commandCtrls.Add(new CommandCtrl(_soCommandDatas[i]));
        }
    }

    public void Inputkey(char key)
    {
        _inputTime = Time.time;

        if (_inputTime - _beInputTime >= _commandInputTime)
            ResetAllCommandList();

        for (int i = 0; i < _commandCtrls.Count; i++)
            _commandCtrls[i].InsertKey(key);

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
            for (int i = 0; i < _commandCtrls.Count; i++)
            {
                if (_commandCtrls[i].CheckCommand() == true)
                {
                    OnCommandCastEvent?.Invoke(_commandCtrls[i].CommandData.skillName, false);
                    break;
                }

                if (_commandCtrls[i].CheckReverseCommand() == true)
                {
                    OnCommandCastEvent?.Invoke(_commandCtrls[i].CommandData.skillName, true);
                    break;
                }
            }
            
            ResetAllCommandList();
        }
        
        _beInputTime = Time.time;
    }

    private void ResetAllCommandList()
    {
        for (int i = 0; i < _commandCtrls.Count; i++)
        {
            _commandCtrls[i].ResetKey();
        }
    }

    public SOCommandData GetCommandData(string skillName)
    {
        int indexForSkillName = _soCommandDatas.FindIndex(e => e.skillName == skillName);
        return _soCommandDatas[indexForSkillName];
    }

    public class CommandCtrl
    {
        private SOCommandData _commandData;

        public SOCommandData CommandData => _commandData;

        private List<char> _commandList;

        private readonly string _commandString;
        private readonly string _reverseCommandString;
        private int _commandCount;
        private int _reversCommandCount;

        //---
        public List<char> CommandList
        {
            get { return _commandList; }
        }

        public string CommandString
        {
            get { return _commandString; }
        }
        public string ReverseCommandString
        {
            get { return _reverseCommandString; }
        }
        //---

        public CommandCtrl(SOCommandData soCommandData)
        {
            _commandData = soCommandData;

            _commandString = CommandData.commandString;
            _reverseCommandString = CommandData.reverseCommandString;
            _commandList = new List<char>();

            _commandCount = 0;
            _reversCommandCount = 0;
            
            Debug.Log("SKill : " + _commandData.skillName + " Reverse : " + _reverseCommandString);
        }

        public void InsertKey(char key)
        {
            _commandList.Add(key);
        }

        public void ResetKey()
        {
            _commandList.Clear();
            _commandCount = 0;
            _reversCommandCount = 0;
        }

        public bool CheckCommand()
        {
            if (_commandList.Count < _commandString.Length)
                return false;

            _commandList = _commandList.GetRange(_commandList.Count - _commandString.Length, _commandString.Length);

            for (int i = 0; i < _commandString.Length; i++)
            {
                if (_commandList[i] == _commandString[i])
                {
                    _commandCount++;
                }
            }

            if (_commandCount == _commandString.Length)
            {
                return true;
            }
            
            return false;
        }

        public bool CheckReverseCommand()
        {
            if (_commandList.Count < _reverseCommandString.Length)
                return false;

            _commandList = _commandList.GetRange(_commandList.Count - _reverseCommandString.Length, _reverseCommandString.Length);

            for (int i = 0; i < _reverseCommandString.Length; i++)
            {
                if (_commandList[i] == _reverseCommandString[i])
                {
                    _reversCommandCount++;
                }
            }

            if (_reversCommandCount == _reverseCommandString.Length)
            {
                return true;
            }
            
            return false;
        }

    }
}
