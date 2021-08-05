using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class SkillDataBank : MonoBehaviour
{
    public static SkillDataBank instance;

    [SerializeField] 
    private List<SkillDataBase> _skillDatabases;

    private List<Dictionary<string, object>> _playerSkillData;

    private Dictionary<string, SkillDataBase> _skillDataBaseDic;

    private GameLoader.SceneLoadCallback _sceneLoadCallback;

    private bool _isInit = false;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        _sceneLoadCallback += SceneLoadCallBack;
    }

    private void Start()
    {
        GameLoader.instance.AddSceneLoadCallback(_sceneLoadCallback);
    }

    private void Init()
    {
        _isInit = true;
        ResetSkillData();
        
        LoadSkillData("PlayerSkillData");

        SettingSkillData();
        SettingSkillDataBaseDic();

    }

    private bool SceneLoadCallBack(out string Error)
    {
        Error = "Test";

        if (_isInit == false)
        {
            Init();
        }

        return true;
    }

    private void ResetSkillData()
    {
        for (int i = 0; i < _skillDatabases.Count; i++)
        {
            _skillDatabases[i].Name = String.Empty;
            _skillDatabases[i].Grade = String.Empty;
            _skillDatabases[i].IsGet = false;
            _skillDatabases[i].Class = String.Empty;
            _skillDatabases[i].CoolTime = 0;
            _skillDatabases[i].IsCasting = false;
            _skillDatabases[i].AttackRange = 0;
            _skillDatabases[i].Target = 0;
            _skillDatabases[i].Description = String.Empty;
            _skillDatabases[i].BuffTime = 0;
            _skillDatabases[i].SkillNumberOfTimes = 0;
            _skillDatabases[i].Damages = new List<int>();
            _skillDatabases[i].HitIntervals = new List<float>();
            _skillDatabases[i].HitNumberOfTimes = new List<int>();
            _skillDatabases[i].KnockBackTimes = new List<float>();
            _skillDatabases[i].KnockBackXs = new List<float>();
            _skillDatabases[i].KnockBackYs = new List<float>();
            _skillDatabases[i].Stiffness = 0;
            _skillDatabases[i].MoveTimes = new List<float>();
            _skillDatabases[i].MoveXs = new List<float>();
            _skillDatabases[i].MoveYs = new List<float>();
            _skillDatabases[i].ProjectileMoveTime = 0;
            _skillDatabases[i].ProjectileMoveX = 0;
            _skillDatabases[i].ProjectileMoveY = 0;
            _skillDatabases[i].StatusEffects = new List<int>();
            _skillDatabases[i].FristDelay = 0;
            _skillDatabases[i].EndDelay = 0;
            _skillDatabases[i].NextSkill = 0;
            _skillDatabases[i].IconName = String.Empty;
        }
    }
    
    private void LoadSkillData(string skillDataCsvName)
    {
        _playerSkillData = CSVReader.Read(skillDataCsvName);
    }

    private void SettingSkillData()
    {
        for (int i = 0; i < _skillDatabases.Count; i++)
        {
            //var _skillDataFindIndex = _skillDatabases.FindIndex(e => e.ID == i);
            
            _skillDatabases[i].Name = _playerSkillData[_skillDatabases[i].ID]["name"].ToString();
            _skillDatabases[i].Grade = _playerSkillData[_skillDatabases[i].ID]["grade"].ToString();
            _skillDatabases[i].IsGet = Convert.ToBoolean(_playerSkillData[_skillDatabases[i].ID]["isGet"]);
            _skillDatabases[i].Class = _playerSkillData[_skillDatabases[i].ID]["class"].ToString();
            _skillDatabases[i].CoolTime =Convert.ToInt32(_playerSkillData[_skillDatabases[i].ID]["coolTime"]);
            _skillDatabases[i].IsCasting =
                Convert.ToBoolean(_playerSkillData[_skillDatabases[i].ID]["isCasting"]);
            _skillDatabases[i].AttackRange =
                Convert.ToInt32(_playerSkillData[_skillDatabases[i].ID]["attackRange"]);
            _skillDatabases[i].CastingTime =
                Convert.ToInt32(_playerSkillData[_skillDatabases[i].ID]["castingTime"]);
            _skillDatabases[i].Target =
                Convert.ToInt32(_playerSkillData[_skillDatabases[i].ID]["target"]);
            _skillDatabases[i].Description = _playerSkillData[_skillDatabases[i].ID]["description"].ToString();
            _skillDatabases[i].BuffTime = (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["buffTime"]);
            _skillDatabases[i].SkillNumberOfTimes =
                Convert.ToInt32(_playerSkillData[_skillDatabases[i].ID]["skillNumberOfTimes"]);

            _skillDatabases[i].Stiffness = (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["stiffness"]);
            _skillDatabases[i].FristDelay = (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["firstDelay"]);
            _skillDatabases[i].EndDelay = (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["endDelay"]);
            _skillDatabases[i].IconName = _playerSkillData[_skillDatabases[i].ID]["icon"].ToString();
            _skillDatabases[i].ProjectileMoveTime =
                (float) Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["projectileMoveTime"]);
            _skillDatabases[i].ProjectileMoveX =
                (float) Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["projectileMoveX"]);
            _skillDatabases[i].ProjectileMoveY =
                (float) Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["projectileMoveY"]);

            var hitNumberOfTimes = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["hitNumberOfTimes"]);
            SplitDataAndPutInlist(_skillDatabases[i].HitNumberOfTimes, hitNumberOfTimes);

            var damages = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["damage"]);
            SplitDataAndPutInlist(_skillDatabases[i].Damages, damages);
            
            var hitInterval = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["hitInterval"]);
            SplitDataAndPutInlist(_skillDatabases[i].HitIntervals, hitInterval);
            
            var knockBackTimes = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["knockBackTime"]);
            SplitDataAndPutInlist(_skillDatabases[i].KnockBackTimes, knockBackTimes);
            
            var knockBackX = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["knockBackX"]);
            SplitDataAndPutInlist(_skillDatabases[i].KnockBackXs, knockBackX);
            
            var knockBackY = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["knockBackY"]);
            SplitDataAndPutInlist(_skillDatabases[i].KnockBackYs, knockBackY);

            var moveTime = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["moveTime"]);
            SplitDataAndPutInlist(_skillDatabases[i].MoveTimes, moveTime);
            
            var moveX = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["moveX"]);
            SplitDataAndPutInlist(_skillDatabases[i].MoveXs, moveX);

            var moveY = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["moveY"]);
            SplitDataAndPutInlist(_skillDatabases[i].MoveYs, moveY);
            
            var statusEffect = Convert.ToString(_playerSkillData[_skillDatabases[i].ID]["statusEffect"]);
            SplitDataAndPutInlist(_skillDatabases[i].StatusEffects, statusEffect);

            _skillDatabases[i].Icon = Addressable.instance.GetSprite(_skillDatabases[i].IconName);
        }
        
    }

    private void SettingSkillDataBaseDic()
    {
        _skillDataBaseDic = new Dictionary<string, SkillDataBase>();

        for (int i = 0; i < _skillDatabases.Count; i++)
        {
            _skillDataBaseDic[_skillDatabases[i].Name] = _skillDatabases[i];
        }
    }

    private void SplitDataAndPutInlist(in List<int> data, string splitData)
    {
        var splitDataToList = splitData.Split('/').ToList();
        
        
        for (int i = 0; i < splitDataToList.Count; i++)
        {
            data.Add(Convert.ToInt32(splitDataToList[i]));
        }
    }
    
    private void SplitDataAndPutInlist(in List<float> data, string splitData)
    {
        var splitDataToList = splitData.Split('/').ToList();

        for (int i = 0; i < splitDataToList.Count; i++)
        {
            data.Add((float)Convert.ToDouble(splitDataToList[i]));
        }
    }

    // 해당 함수로 랜덤한 데이터를 가져올 때 중복인지 체크가 외부에서 필요함.
    public SkillDataBase GetRandomSKillDataBase()
    {
        int randIndex = UnityEngine.Random.Range(0, _skillDatabases.Count);

        return _skillDatabases[randIndex];
    }
    

    public SkillDataBase GetSkillData(string skillName)
    {
        if (_skillDataBaseDic.ContainsKey(skillName) == false)
        {
            Debug.LogError("is Not Skill Data in Dic");
            return null;
        }

        return _skillDataBaseDic[skillName];
    }
    
    

}
