using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
    private float _commandInputTime = 5.0f;
    public float CommandInputTime => _commandInputTime;

    [SerializeField] 
    private int[] _slotInSkillLifeTimeArr;
    
    private float _inputTime;
    private float _beInputTime;
    
    
    public event UnityAction<TransitionCondition, bool> OnCommandCastEvent;

    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        CreateSkillSlotAndAddList();
        GameManager.instance.AddMapEndEventListener(MapEndEventCall);
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
            var newSlot = new SkillSlot(_skillSlotCommandDatas[i], this, i);
            _skillSlots.Add(newSlot);
            _skillSlots[i].SkillLifeTime = _slotInSkillLifeTimeArr[i];
            GameManager.instance.UIMng.UIPlayerInfo.SkillInfo.RegistSkillData(i, newSlot);
        }

        //test code
        for (int i = 0; i < _skillSlots.Count; i++)
        {
            _skillSlots[i].InsertSkillDataBase(_skillDataBases[i]);
            //GameManager.instance.UIMng.UIPlayerInfo.SkillInfo.UpdateSkillData(i);
        }
        
        _skillSlots[0].RockSlot();

    }

    /*
     * 고재협 수정 : 반환형 void -> bool
     * 여기서 슬롯의 교체 가능 여부(잠금등)을 체크해서 불가시 false를 반환해줘야 할듯
     */
    // 슬롯 인덱스와 스킬 데이터를 받아서 삽입.
    public bool InsertSkillBaseInSkillSlot(SkillDataBase skillDataBase, int slotIndex)
    {
        _skillSlots[slotIndex].InsertSkillDataBase(skillDataBase);
        return true;
    }

    // 스킬이 사라지지 않도록 Rock.
    public void RockSkillSlot(int slotIndex)
    {
        _skillSlots[slotIndex].RockSlot();
    }

    
    // 랜덤한 하나의 슬롯을 봉인.
    public void SealSkillSlot()
    {
        var isDone = false;
        while (!isDone)
        {
            int slotIndex = Random.Range(0, _skillSlots.Count);

            if (_skillSlots[slotIndex].IsSeal == false)
            {
                _skillSlots[slotIndex].SealSlot();
                isDone = true;
            }
        }
    }

    public void UnSealingSkillSlot(int slotIndex)
    {
        _skillSlots[slotIndex].UnSealing();
    }
    
    // 1. 커맨드 결정키는 마지막 단 하나
    // LR Z
    // 2. 반드시 모든 스킬 커맨드에는 결정키가 하나 포함된다
    // 방향키로만 이루어진 스킬은 X
    // 3. 중복되는 커맨드는 존재하지 않음
    // LRZ   LRZLX XX
    // 던파에서 커맨드를 저장 위위 아래아래 Z 스킬 위위 아래 z
    // 중간에 다른 키가 입력됐을 때 스킬이 발동되는 경우?
    public void Inputkey(char key)
    {
        _inputTime = Time.time;

        if (_inputTime - _beInputTime >= _commandInputTime)
        {
            ResetAllCommandList();
        }

        for (int i = 0; i < _skillSlots.Count; i++)
            _skillSlots[i].InsertKey(key);
        
        _beInputTime = Time.time;

        DebugSlotInKeys();
    }

    public bool EnterDecisionKey(char key)
    {
        Inputkey(key);
        
        if (key == 'Z' || key == 'X' || key == 'S') // 커맨드 결정 키
        {
            for (int i = 0; i < _skillSlots.Count; i++)
            {
                if (_skillSlots[i].IsSeal == true)
                {
                    continue;
                }
                
                if (_skillSlots[i].CheckCommand() == true)
                {
                    ResetAllCommandList();
                    return true;
                }

                if (_skillSlots[i].CheckReverseCommand() == true)
                {
                    ResetAllCommandList();
                    return true;
                }
            }
            
        }
        
        ResetAllCommandList();

        return false;
    }
    
    private void ResetAllCommandList()
    {
        for (int i = 0; i < _skillSlots.Count; i++)
        {
            _skillSlots[i].ResetKey();
        }
    }

    private void MapEndEventCall()
    {
        for (int i = 0; i < _skillSlotNumber; i++)
        {
            _skillSlots[i].MapEndEventCall();
        }
    }

    private void DebugSlotInKeys()
    {
        string commandString = String.Empty;
        for (int i = 0; i < _skillSlots.Count; i++)
        {
            commandString = i + " slot";
            for (int j = 0; j < _skillSlots[i].CommandList.Count; j++)
            {
                commandString += " " + _skillSlots[i].CommandList[j];
            }
            
            commandString = String.Empty;
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
        private int _slotIndex;
        private bool _isActiveSkillSlot;
        private float _skillSlotCoolTime;
        private float _skillSlotCurCoolTime;
        private bool _isRock;
        private bool _isSeal;
        public bool IsSeal => _isSeal;

        public float SkillSlotCoolTime => _skillSlotCoolTime;

        public float SkillSlotCurCoolTime
        {
            get => _skillSlotCurCoolTime;
            set => _skillSlotCurCoolTime = value;
        }

        private int _skillLifeTime;

        public int SkillLifeTime
        {
            get => _skillLifeTime;
            set => _skillLifeTime = value;
        }

        private int _curSkillLifeTime;

        public SkillSlot(SOCommandData soCommandData, SkillSlotManager skillSlotManager, int slotIndex)
        {
            _skillSlotManager = skillSlotManager;
            
            _commandData = soCommandData;

            _commandString = CommandData.commandString;
            _reverseCommandString = CommandData.reverseCommandString;
            _commandList = new List<char>();
            _isActiveSkillSlot = false;
            _isRock = false;
            _isSeal = false;
            _slotIndex = slotIndex;
            
            _skillSlotCoolTime = 0.0f;
            _skillSlotCurCoolTime = 0.0f;
            _commandCount = 0;
            _reversCommandCount = 0;
            _curSkillLifeTime = 0;

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

        public void SealSlot()
        {
            _isSeal = true;
        }

        public void UnSealing()
        {
            _isSeal = false;
        }

        public void InsertSkillDataBase(SkillDataBase skillDataBase)
        {
            _skillDataBase = skillDataBase;
            _skillSlotCoolTime = _skillDataBase.CoolTime;
            _skillSlotCurCoolTime = _skillSlotCoolTime;
            _isActiveSkillSlot = true;
            GameManager.instance.UIMng.UIPlayerInfo.SkillInfo.UpdateSkillData(_slotIndex);
        }

        public void RemoveSkillDataBase()
        {
            _skillSlotCurCoolTime = 0.0f;
            _skillSlotCoolTime = 0.0f;
            _curSkillLifeTime = 0;

            _skillDataBase = null;
            _isActiveSkillSlot = false;
            GameManager.instance.UIMng.UIPlayerInfo.SkillInfo.UpdateSkillData(_slotIndex);
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
            if (_isActiveSkillSlot == false || _skillDataBase == null)
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
                _skillSlotManager.OnCommandCastEvent?.Invoke(_skillDataBase.SkillCondition, false);
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
                _skillSlotManager.OnCommandCastEvent?.Invoke(_skillDataBase.SkillCondition, true);
                _skillSlotManager.StartCoroutine(SlotCoolTimeCoroutine());
                return true;
            }
            
            return false;
        }

        public void MapEndEventCall()
        {
            if (_skillDataBase == null || _isRock)
            {
                return;
            }

            _curSkillLifeTime++;
            
            if(_curSkillLifeTime >= _skillLifeTime)
                RemoveSkillDataBase();
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
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                _skillSlotCurCoolTime = timer;
                yield return new WaitForFixedUpdate();
            }
            
            _isActiveSkillSlot = true;
        }

    }
}
