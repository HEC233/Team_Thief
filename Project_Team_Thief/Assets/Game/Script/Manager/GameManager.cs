using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Util.Tile;
<<<<<<< HEAD
using PS.FX;
=======
using PS.Shadow;
>>>>>>> particleTest

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CameraManager cameraMng;
    public TimeManager timeMng;
    public SoundManager soundMng;
    public UIManager uiMng;
<<<<<<< HEAD
    public EffectSystem FX;
    
=======

    public ShadowParticleSystem shadow;

>>>>>>> particleTest
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
        //Application.targetFrameRate = 60;
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
