using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    private IActor controlUnit = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.anyKey)
        {
            controlUnit.Transition(TransitionCondition.Idle);
        }
        else if (Input.GetKey(KeyCode.C))
        {
            controlUnit.Transition(TransitionCondition.Jump);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            controlUnit.Transition(TransitionCondition.LeftMove);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            controlUnit.Transition(TransitionCondition.RightMove);
        }
    }

    public void SetControlUnit(IActor unit)
    {
        controlUnit = unit;
        //Debug.Log("Is On");
    }
}
