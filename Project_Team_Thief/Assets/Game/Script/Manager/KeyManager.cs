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
        if (Input.GetKey(KeyCode.C))
        {
            controlUnit.Transition(TransitionCondition.Jump);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            controlUnit.Transition(TransitionCondition.LeftMove);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            controlUnit.Transition(TransitionCondition.RightMove);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            controlUnit.Transition(TransitionCondition.Dash);
        }
        if (Input.GetKey(KeyCode.UpArrow))
            controlUnit.Transition(TransitionCondition.Wallslideing);
        if (Input.GetKeyDown(KeyCode.X))
            controlUnit.Transition(TransitionCondition.Attack);


    }

    public void SetControlUnit(IActor unit)
    {
        controlUnit = unit;
        //Debug.Log("Is On");
    }

    public IActor GetControlActor()
    {
        return controlUnit;
    }
}
