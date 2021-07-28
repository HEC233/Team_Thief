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

    private Dictionary<string, SkillDataBase> _SkillDataBaseDic;

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
            var _skillDataFindIndex = _skillDatabases.FindIndex(e => e.ID == i);

            _skillDatabases[_skillDataFindIndex].Name = _playerSkillData[i]["name"].ToString();
            _skillDatabases[_skillDataFindIndex].Grade = _playerSkillData[i]["grade"].ToString();
            _skillDatabases[_skillDataFindIndex].IsGet = Convert.ToBoolean(_playerSkillData[i]["isGet"]);
            //_skillDatabases[_skillDataFindIndex].Class = _playerSkillData[i]["class"].ToString();
            _skillDatabases[_skillDataFindIndex].CoolTime =Convert.ToInt32(_playerSkillData[i]["coolTime"]);
            _skillDatabases[_skillDataFindIndex].IsCasting =
                Convert.ToBoolean(_playerSkillData[i]["isCasting"]);
            _skillDatabases[_skillDataFindIndex].AttackRange =
                Convert.ToInt32(_playerSkillData[i]["attackRange"]);
            _skillDatabases[_skillDataFindIndex].CastingTime =
                Convert.ToInt32(_playerSkillData[i]["castingTime"]);
            _skillDatabases[_skillDataFindIndex].Target =
                Convert.ToInt32(_playerSkillData[i]["target"]);
            _skillDatabases[_skillDataFindIndex].Description = _playerSkillData[i]["description"].ToString();
            _skillDatabases[_skillDataFindIndex].BuffTime = (float)Convert.ToDouble(_playerSkillData[i]["buffTime"]);
            _skillDatabases[_skillDataFindIndex].SkillNumberOfTimes =
                Convert.ToInt32(_playerSkillData[i]["skillNumberOfTimes"]);

            _skillDatabases[_skillDataFindIndex].Stiffness = (float)Convert.ToDouble(_playerSkillData[i]["stiffness"]);
            _skillDatabases[_skillDataFindIndex].FristDelay = (float)Convert.ToDouble(_playerSkillData[i]["fristDelay"]);
            _skillDatabases[_skillDataFindIndex].EndDelay = (float)Convert.ToDouble(_playerSkillData[i]["endDelay"]);
            _skillDatabases[_skillDataFindIndex].Icon = _playerSkillData[i]["icon"].ToString();


            
            for (int j = 0; j < 3; j++)
            {
                _skillDatabases[_skillDataFindIndex].Damages[i] =
                    Convert.ToInt32(_playerSkillData[i]["damage" + (i + 1)]);
                
                _skillDatabases[_skillDataFindIndex].HitIntervals[i] =
                    (float)Convert.ToDouble(_playerSkillData[i]["hitInterval" + (i + 1)]);
                
                // _skillDatabases[_skillDataFindIndex].HitNumberOfTimes[i] =
                //     Convert.ToInt32(_playerSkillData[i]["hitNumberOfTime" + (i + 1)]);
                
                _skillDatabases[_skillDataFindIndex].KnockBackTimes[i] =
                    (float)Convert.ToDouble(_playerSkillData[i]["knockBackTime" + (i + 1)]);
                
                _skillDatabases[_skillDataFindIndex].KnockBackXs[i] =
                    (float)Convert.ToDouble(_playerSkillData[i]["knockBackX" + (i + 1)]);
                
                _skillDatabases[_skillDataFindIndex].KnockBackYs[i] =
                    (float)Convert.ToDouble(_playerSkillData[i]["knockBackY" + (i + 1)]);
                
                _skillDatabases[_skillDataFindIndex].MoveTimes[i] =
                    (float)Convert.ToDouble(_playerSkillData[i]["moveTime" + (i + 1)]);
                
                _skillDatabases[_skillDataFindIndex].MoveXs[i] =
                    (float)Convert.ToDouble(_playerSkillData[i]["moveX" + (i + 1)]);
                
                _skillDatabases[_skillDataFindIndex].MoveYs[i] =
                    (float)Convert.ToDouble(_playerSkillData[i]["moveY" + (i + 1)]);
            }

            for (int j = 0; j < 4; j++)
            {
                _skillDatabases[_skillDataFindIndex].StatusEffects[i] =
                    Convert.ToInt32(_playerSkillData[i]["statusEffect" + (i + 1)]);
            }


        }
        
        
    }

}
