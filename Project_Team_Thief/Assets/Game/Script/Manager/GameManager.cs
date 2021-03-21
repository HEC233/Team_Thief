using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CameraManager cameraMng = new CameraManager();
    public TimeManager timeMng = new TimeManager();
    public SoundManager soundMng = new SoundManager();
    public UIManager uiMng = new UIManager();
    
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
        
        TileCoordClass.SetGrid(grid);
    }
    
    public void SetControlUnit(IActor unit)
    {
        _keyManger.SetControlUnit(unit);
    }
}
