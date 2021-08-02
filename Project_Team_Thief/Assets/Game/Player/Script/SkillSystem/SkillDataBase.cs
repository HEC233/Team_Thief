using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public abstract class SkillDataBase : ScriptableObject
{
    [SerializeField]
    private int _id;
    public int ID => _id;

    
    [SerializeField, Tooltip("스킬 사용 가능 횟수")] 
    private int _numberOfTimesTheSkill; // 스킬 사용 가능 횟수.
    public int NumberOfTimesTheSkill => _numberOfTimesTheSkill;

    [SerializeField] 
    private SignalSourceAsset _cinemachineSignalSource;
    public SignalSourceAsset CinemachineSignalSource => _cinemachineSignalSource;

    // [SerializeField] 
    // private string _skillName;
    //
    // public string SkillName => _skillName;
    //
    // [SerializeField] 
    // private float _increaseEncroachment;
    //
    // public float IncreaseEncroachment => _increaseEncroachment;
    //
    // [SerializeField] 
    // private float _encroachmentDecrease;
    //
    // public float DecreaseEncroachment => _encroachmentDecrease;
    
    //New Data -> skill Info
    [SerializeField]
    private string _name;
    public string Name
    {
        get => _name;
        set => _name = value;
    }
    
    [SerializeField]
    private string _grade;
    
    public string Grade
    {
        get => _grade;
        set => _grade = value;
    }

    [SerializeField]
    private bool _isGet;

    public bool IsGet
    {
        get => _isGet;
        set => _isGet = value;
    }
    
    [SerializeField]
    private string _class;
    
    public string Class
    {
        get => _class;
        set => _class = value;
    }
    
    [SerializeField]
    private float _coolTime;

    public float CoolTime
    {
        get => _coolTime;
        set => _coolTime = value;
    }

    private bool _isCasting;

    public bool IsCasting
    {
        get => _isCasting;
        set => _isCasting = value;
    }
    
    private int _castingTime;

    public int CastingTime
    {
        get => _castingTime;
        set => _castingTime = value;
    }
    
    private int _attackRange;

    public int AttackRange
    {
        get => _attackRange;
        set => _attackRange = value;
    }

    private int _target;

    public int Target
    {
        get => _target;
        set => _target = value;
    }

    [SerializeField]
    private string _description;

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    private float _buffTime;

    public float BuffTime
    {
        get => _buffTime;
        set => _buffTime = value;
    }

    private int _skillNumberOfTimes;

    public int SkillNumberOfTimes
    {
        get => _skillNumberOfTimes;
        set => _skillNumberOfTimes = value;
    }

    //New Data -> Skill Attack Info
    private List<int> _damages;

    public List<int> Damages
    {
        get => _damages;
        set => _damages = value;
    }

    private List<float> _hitIntervals;

    public List<float> HitIntervals
    {
        get => _hitIntervals;
        set => _hitIntervals = value;
    }

    private List<int> _hitNumberOfTimes;

    public List<int> HitNumberOfTimes
    {
        get => _hitNumberOfTimes;
        set => _hitNumberOfTimes = value;
    }

    private List<float> _knockBackTimes;

    public  List<float> KnockBackTimes
    {
        get => _knockBackTimes;
        set => _knockBackTimes = value;
    }

    private List<float> _knockBackXs;

    public  List<float> KnockBackXs
    {
        get => _knockBackXs;
        set => _knockBackXs = value;
    }

    private List<float> _knockBackYs;
    
    public List<float> KnockBackYs
    {
        get => _knockBackYs;
        set => _knockBackYs = value;
    }
    private float _stiffness;

    public float Stiffness
    {
        get => _stiffness;
        set => _stiffness = value;
    }

    //New Data -> Skill Move Info
    private List<float> _moveTimes;

    public  List<float> MoveTimes
    {
        get => _moveTimes;
        set => _moveTimes = value;
    }

    private List<float> _moveXs;

    public  List<float> MoveXs
    {
        get => _moveXs;
        set => _moveXs = value;
    }

    private List<float> _moveYs;

    public  List<float> MoveYs
    {
        get => _moveYs;
        set => _moveYs = value;
    }

    private float projectileMoveTime;

    public float ProjectileMoveTime
    {
        get => projectileMoveTime;
        set => projectileMoveTime = value;
    }

    private float projectileMoveX;

    public float ProjectileMoveX
    {
        get => projectileMoveX;
        set => projectileMoveX = value;
    }
    
    private float projectileMoveY;

    public float ProjectileMoveY
    {
        get => projectileMoveY;
        set => projectileMoveY = value;
    }
    
    //New Data -> Skill Effect Info
    private List<int> _statusEffects;

    public List<int> StatusEffects
    {
        get => _statusEffects;
        set => _statusEffects = value;
    }

    //New Data -> Skill Delay Info
    private float _fristDelay;

    public float FristDelay
    {
        get => _fristDelay;
        set => _fristDelay = value;
    }

    private float _endDelay;

    public float EndDelay
    {
        get => _endDelay;
        set => _endDelay = value;
    }

    private float nextSkill;

    public float NextSkill
    {
        get => nextSkill;
        set => nextSkill = value;
    }

    //New Data -> Etc
    private string _icon;

    public string Icon
    {
        get => _icon;
        set => _icon = value;
    }

    // Unit까지 넣어주자.
    public abstract SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit);
}
