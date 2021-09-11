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
        if (controlUnit == null)
            return;
        

        List<KeyCode> pressedInput = new List<KeyCode>();

        if (GameManager.instance.PlayerActor != controlUnit)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                controlUnit.Transition(TransitionCondition.MouseMove);
            }
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                controlUnit.Transition(TransitionCondition.ArrowInput);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                controlUnit.Transition(TransitionCondition.DialogueNext);
            }
            if(Input.GetKeyDown(KeyCode.C))
            {
                controlUnit.Transition(TransitionCondition.DialogueSkip);
            }
            if(Input.GetKeyUp(KeyCode.C))
            {
                controlUnit.Transition(TransitionCondition.DialogueSkipCancel);
            }
        }

        if (GameManager.instance.GameState != GameStateEnum.InGame)
        {
            return;
        }

        if (GameManager.instance.PlayerActor != controlUnit)
        {
            return;
        }

        if (Input.anyKey || Input.anyKeyDown)
        {
            if (GameManager.instance.isPlayerDead)
            {
                //GameManager.instance.ExitToMainMenu();
                return;
            }

            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code))
                {
                    m_activeInputs.Remove(code);
                    m_activeInputs.Add(code);
                    pressedInput.Add(code);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.instance.PauseGame();
            }
            if(Input.GetKeyDown(KeyCode.BackQuote))
            {
                GameManager.instance.UIMng.ToggleDeveloperConsole();
            }

            if(GameManager.instance.GameState != GameStateEnum.InGame)
            {
                return;
            }
        
            if (Input.GetKey(KeyCode.C))
            {
                controlUnit.Transition(TransitionCondition.Jump);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
                controlUnit.Transition(TransitionCondition.LeftMove);

            if (Input.GetKey(KeyCode.RightArrow))
                controlUnit.Transition(TransitionCondition.RightMove);

            // one way 타일 뒤집기 부분
            if(Input.GetKey(KeyCode.DownArrow))
            {
                GameManager.instance.SetOnewayTile(false);
            }
            else
            {
                GameManager.instance.SetOnewayTile(true);
            }
            //GameManager.instance.SetOnewayTile(!Input.GetKey(KeyCode.DownArrow));

            if (Input.GetKeyDown(KeyCode.Z))
                controlUnit.Transition(TransitionCondition.Dash);

            if (Input.GetKey(KeyCode.UpArrow))
                controlUnit.Transition(TransitionCondition.Wallslideing);


            if (Input.GetKeyDown(KeyCode.X))
            {
                if (GameManager.instance.SkillSlotMng.EnterDecisionKey('X') == false)
                {
                    controlUnit.Transition(TransitionCondition.Attack);
                }
            }

            if (Input.GetKeyDown(KeyCode.Z))
                controlUnit.Transition(TransitionCondition.Action);

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
                GameManager.instance.SkillSlotMng.Inputkey('L');
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                GameManager.instance.SkillSlotMng.Inputkey('R');
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                GameManager.instance.SkillSlotMng.Inputkey('U');
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                GameManager.instance.SkillSlotMng.Inputkey('D');
            else if (Input.GetKeyDown(KeyCode.Z))
                GameManager.instance.SkillSlotMng.Inputkey('Z');
            else if (Input.GetKeyDown(KeyCode.Space))
                GameManager.instance.SkillSlotMng.Inputkey('S');
            /// 스킬 입력 처리
        }
        else
        {
            controlUnit.Transition(TransitionCondition.None);
        }
        
    }

    public void SetControlActor(IActor unit)
    {
        controlUnit = unit;
        //Debug.Log("Is On");
    }

    public IActor GetControlActor()
    {
        return controlUnit;
    }
}
