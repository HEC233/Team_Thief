using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
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
