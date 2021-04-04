//https://answers.unity.com/questions/1069586/checking-if-any-key-is-up.html
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    private IActor controlUnit = null;
    private List<KeyCode> m_activeInputs = new List<KeyCode>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<KeyCode> pressedInput = new List<KeyCode>();
        
        if (Input.anyKey || Input.anyKeyDown)
        {
            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code))
                {
                    m_activeInputs.Remove(code);
                    m_activeInputs.Add(code);
                    pressedInput.Add(code);
                }
            }

            if (Input.GetKey(KeyCode.C))
                controlUnit.Transition(TransitionCondition.Jump);
            
            if (Input.GetKey(KeyCode.LeftArrow))
                controlUnit.Transition(TransitionCondition.LeftMove);

            if (Input.GetKey(KeyCode.RightArrow))
                controlUnit.Transition(TransitionCondition.RightMove);

            if (Input.GetKeyDown(KeyCode.Space))
                controlUnit.Transition(TransitionCondition.Dash);
            
            if (Input.GetKey(KeyCode.UpArrow))
                controlUnit.Transition(TransitionCondition.Wallslideing);
            
            if (Input.GetKeyDown(KeyCode.X))
                controlUnit.Transition(TransitionCondition.Attack);

            List<KeyCode> releasedInput = new List<KeyCode>();

            foreach (KeyCode code in m_activeInputs)
            {
                releasedInput.Add(code);

                if (!pressedInput.Contains(code))
                {
                    releasedInput.Remove(code);
                    
                    if (code == KeyCode.LeftArrow || code == KeyCode.RightArrow)
                        controlUnit.Transition(TransitionCondition.StopMove);
                }
            }

            m_activeInputs = releasedInput;
        }
        else
        {
            controlUnit.Transition(TransitionCondition.Idle);
        }
        

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
