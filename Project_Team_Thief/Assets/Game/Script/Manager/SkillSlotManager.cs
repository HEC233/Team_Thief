using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillSlotManager : MonoBehaviour
{
    [SerializeField]
    private int _skillSlotNumber;

    [SerializeField] 
    private List<SOCommandData> _skillSlotCommandDatas;
    
    [SerializeField] 
    private List<SkillDataBase> _skillDataBases;

    private List<SkillSlot> _skillSlots = new List<SkillSlot>();
    
    public List<SkillSlot> SkillSlots => _skillSlots;
    
    [SerializeField]

    private float _commandInputTime = 0.1f;

    private float _inputTime;
    private float _beInputTime;
    
    public event UnityAction<string, bool> OnCommandCastEvent;

    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        CreateSkillSlotAndAddList();
    }

    private void CreateSkillSlotAndAddList()
    {
        _skillSlots = new List<SkillSlot>();

        if (_skillSlotNumber != _skillSlotCommandDatas.Count)
        {
            Debug.LogError("Is Not Same skillSlotCount and SkillSlotCommandData");
            return;
        }
        
        for (int i = 0; i < _skillSlotNumber; i++)
        {
            _skillSlots.Add(new SkillSlot(_skillSlotCommandDatas[i], this));
        }
        
        //test code
        for (int i = 0; i < _skillSlots.Count; i++)
        {
            _skillSlots[i].InsertSkillDataBase(_skillDataBases[i]);
        }
    }

    // 왼쪽부터 순서대로 빈 공간에 스킬 삽입
    public void InsertSkillBaseInSkillSlot(SkillDataBase skillDataBase)
    {
        for (int i = 0; i < _skillSlots.Count; i++)
        {
            if (_skillSlots[i].SkillDataBase == null)
            {
                _skillSlots[i].InsertSkillDataBase(skillDataBase);
                break;
            }
        }
    }

    public void RockSkillSlot(int slotIndex)
    {
        _skillSlots[slotIndex].RockSlot();
    }
    
    public void Inputkey(char key)
    {
        _inputTime = Time.time;

        if (_inputTime - _beInputTime >= _commandInputTime)
            ResetAllCommandList();

        for (int i = 0; i < _skillSlots.Count; i++)
            _skillSlots[i].InsertKey(key);

        // 1. 커맨드 결정키는 마지막 단 하나
        // LR Z
        // 2. 반드시 모든 스킬 커맨드에는 결정키가 하나 포함된다
        // 방향키로만 이루어진 스킬은 X
        // 3. 중복되는 커맨드는 존재하지 않음
        // LRZ   LRZLX XX
        // 던파에서 커맨드를 저장 위위 아래아래 Z 스킬 위위 아래 z
        // 중간에 다른 키가 입력됐을 때 스킬이 발동되는 경우?

        if (key == 'Z' || key == 'X' || key == 'S') // 커맨드 결정 키
        {
            for (int i = 0; i < _skillSlots.Count; i++)
            {
                if (_skillSlots[i].CheckCommand() == true)
                {
                    break;
                }

                if (_skillSlots[i].CheckReverseCommand() == true)
                {
                    break;
                }
            }
            
            ResetAllCommandList();
        }
        
        _beInputTime = Time.time;
    }
    
    private void ResetAllCommandList()
    {
        for (int i = 0; i < _skillSlots.Count; i++)
        {
            _skillSlots[i].ResetKey();
        }
    }
    
    public class SkillSlot
    {
        private SkillSlotManager _skillSlotManager;
        
        private SOCommandData _commandData;

        public SOCommandData CommandData => _commandData;
        
        private SkillDataBase _skillDataBase;

        public SkillDataBase SkillDataBase => _skillDataBase;

        private List<char> _commandList;

        private readonly string _commandString;
        private readonly string _reverseCommandString;
        private int _commandCount;
        private int _reversCommandCount;
        private bool _isActiveSkillSlot;
        private float _skillSlotCoolTime;
        private float _skillSlotCurCoolTime;
        private bool _isRock;
        
        public float SkillSlotCurCoolTime
        {
            get => _skillSlotCurCoolTime;
            set => _skillSlotCurCoolTime = value;
        }

        public SkillSlot(SOCommandData soCommandData, SkillSlotManager skillSlotManager)
        {
            _skillSlotManager = skillSlotManager;
            
            _commandData = soCommandData;

            _commandString = CommandData.commandString;
            _reverseCommandString = CommandData.reverseCommandString;
            _commandList = new List<char>();
            _isActiveSkillSlot = false;
            _isRock = false;

            _skillSlotCoolTime = 0.0f;
            _skillSlotCurCoolTime = 0.0f;
            _commandCount = 0;
            _reversCommandCount = 0;
            
        }
        
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

        public void RockSlot()
        {
            _isRock = true;
        }

        public void InsertSkillDataBase(SkillDataBase skillDataBase)
        {
            _skillDataBase = skillDataBase;
            _skillSlotCoolTime = _skillDataBase.CoolTime;
            _isActiveSkillSlot = true;
        }

        public void RemoveSkillDataBase()
        {
            if(_isRock)
                return;

            _skillSlotCurCoolTime = 0.0f;
            _skillSlotCoolTime = 0.0f;

            _skillDataBase = null;
            _isActiveSkillSlot = false;
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
            if (_isActiveSkillSlot == false)
            {
                return false;
            }

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
                _skillSlotManager.OnCommandCastEvent?.Invoke(_skillDataBase.Name, false);
                _skillSlotManager.StartCoroutine(SlotCoolTimeCoroutine());
                return true;
            }
            
            return false;
        }

        public bool CheckReverseCommand()
        {
            if (_isActiveSkillSlot == false)
            {
                return false;
            }
            
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
                _skillSlotManager.OnCommandCastEvent?.Invoke(_skillDataBase.Name, true);
                _skillSlotManager.StartCoroutine(SlotCoolTimeCoroutine());
                return true;
            }
            
            return false;
        }

        IEnumerator SlotCoolTimeCoroutine()
        {
            if (_isActiveSkillSlot == false)
            {
                yield break;
            }

            _isActiveSkillSlot = false;
            float timer = 0.0f;
            _skillSlotCurCoolTime = 0.0f;
            
            while (timer <= _skillSlotCoolTime)
            {
                timer += GameManager.instance.timeMng.FixedDeltaTime;
                _skillSlotCurCoolTime = timer;
                yield return new WaitForFixedUpdate();
            }
            
            _isActiveSkillSlot = true;
        }

    }
}
