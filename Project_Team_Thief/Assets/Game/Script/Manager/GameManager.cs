using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Util.Tile;
using PS.FX;
using PS.Shadow;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CameraManager cameraMng;
    public TimeManager timeMng;
    public SoundManager soundMng;
    public UIManager uiMng;
    public EffectSystem FX;
    public GameSkillMgr GameSkillMgr;

    public CommandManager commandManager;
    

    public ShadowParticleSystem shadow;

    [SerializeField] 
    private KeyManager _keyManger;
    public Grid grid;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Application.targetFrameRate = 60;
        TileCoordClass.SetGrid(grid);
    }
    
    public void SetControlUnit(IActor unit)
    {
        _keyManger.SetControlUnit(unit);
    }

    public IActor GetControlActor()
    {
        return _keyManger.GetControlActor();
    }
}
