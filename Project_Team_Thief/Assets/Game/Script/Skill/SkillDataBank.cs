using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            _skillDatabases[i].Icon = _playerSkillData[_skillDatabases[i].ID]["icon"].ToString();


            
            for (int j = 0; j < 3; j++)
            {
                _skillDatabases[i].Damages[j] =
                    Convert.ToInt32(_playerSkillData[_skillDatabases[i].ID]["damage" + (j + 1)]);
                
                _skillDatabases[i].HitIntervals[j] =
                    (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["hitInterval" + (j + 1)]);
                
                 _skillDatabases[i].HitNumberOfTimes[j] =
                     Convert.ToInt32(_playerSkillData[_skillDatabases[i].ID]["hitNumberOfTimes" + (j + 1)]);
                
                _skillDatabases[i].KnockBackTimes[j] =
                    (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["knockBackTime" + (j + 1)]);
                
                _skillDatabases[i].KnockBackXs[j] =
                    (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["knockBackX" + (j + 1)]);
                
                _skillDatabases[i].KnockBackYs[j] =
                    (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["knockBackY" + (j + 1)]);
                
                _skillDatabases[i].MoveTimes[j] =
                    (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["moveTime" + (j + 1)]);
                
                _skillDatabases[i].MoveXs[j] =
                    (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["moveX" + (j + 1)]);
                
                _skillDatabases[i].MoveYs[j] =
                    (float)Convert.ToDouble(_playerSkillData[_skillDatabases[i].ID]["moveY" + (j + 1)]);
            }

            for (int j = 0; j < 4; j++)
            {
                _skillDatabases[i].StatusEffects[j] =
                    Convert.ToInt32(_playerSkillData[_skillDatabases[i].ID]["statusEffect" + (j + 1)]);
            }

        }
        
    }

    private void SettingSkillDataBaseDic()
    {
        _skillDataBaseDic = new Dictionary<string, SkillDataBase>();

        for (int i = 0; i < _skillDatabases.Count; i++)
        {
            var _skillDataFindIndex = _skillDatabases.FindIndex(e => e.ID == i);

            _skillDataBaseDic[_skillDatabases[_skillDataFindIndex].Name] = _skillDatabases[i];
        }
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
