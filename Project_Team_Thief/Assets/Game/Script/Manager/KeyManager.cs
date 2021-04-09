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
            {
                controlUnit.Transition(TransitionCondition.Jump);
            }
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
                     //controlUnit.Transition(TransitionCondition.Idle);
                 }
             }

              m_activeInputs = releasedInput;
              
              /// 스킬 입력 처리
              if (Input.GetKeyDown(KeyCode.LeftArrow))
                  GameManager.instance.skillMgr.Inputkey('L');
              else if (Input.GetKeyDown(KeyCode.RightArrow))
                  GameManager.instance.skillMgr.Inputkey('R');
              else if (Input.GetKeyDown(KeyCode.Z))
                  GameManager.instance.skillMgr.Inputkey('Z');
              else if (Input.GetKeyDown(KeyCode.X))
                  GameManager.instance.skillMgr.Inputkey('X');
              else if (Input.GetKeyDown(KeyCode.C))
                  GameManager.instance.skillMgr.Inputkey('C');
              else if (Input.GetKeyDown(KeyCode.Space))
                  GameManager.instance.skillMgr.Inputkey('S');
              /// 스킬 입력 처리
        }
         else
         {
             controlUnit.Transition(TransitionCondition.None);
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
