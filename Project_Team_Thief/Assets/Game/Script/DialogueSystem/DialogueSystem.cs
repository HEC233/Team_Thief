using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    private DialogueUIController ui;

    DialogueData _data;
    string[] dialogues;
    int curDialogueIndex;
    Dictionary<string, int> indexKeys;
    byte[][] code;
    int curRuningCodeIndex;

    private bool bAutoPass = false;
    private int PC;

    private bool bInitialized = false;
    private bool bCodeRuning = false;

    private bool bSkipping = false;
    private float skipTimeCheck = 0;

    public bool CheckRunning() { return bCodeRuning || bPaused; }


    InputProcessActor inputProcess;

    public bool InitializeData(ref string ErrorMessage)
    {
        ui?.SetShowDialogue(false);
        
        bInitialized = false;

        var dataObject = GameObject.Find("DialogueData");
        if (dataObject == null)
        {
            ErrorMessage = "DialogueData Object Does not exist.";
            return false;
        }

        var data = dataObject.GetComponent<DialogueData>();
        if (data == null)
        {
            ErrorMessage = "DialogueData Component Does not exist.";
            return false;
        }

        _data = data;
        indexKeys = new Dictionary<string, int>();

        TextAsset text = Addressable.instance.GetText(_data.dialogueName);
        if (text == null)
        {
            ErrorMessage = "DialogueData is invalid. there is no " + _data.dialogueName + " in addresable.";
            return false;
        }
        if (!new DialogueMaker().MakeDialogueScript(text.text, out dialogues, out indexKeys))
        {
            ErrorMessage = "Failed making dailogue script.";
            return false;
        }
        code = new byte[_data.bytecodeName.Length][];
        for (int i = 0; i < _data.bytecodeName.Length; i++)
        {
            TextAsset bytecode = Addressable.instance.GetText(_data.bytecodeName[i]);
            if (bytecode == null)
            {
                ErrorMessage = "DialogueData is invalid. there is no " + _data.bytecodeName[i] + "in addresable.";
                return false;
            }
            code[i] = bytecode.bytes;
        }
        PC = 0;
        bInitialized = true;
        bCodeRuning = false;

        ui = GameManager.instance?.UIMng.uiDialogue;
        ui.SetShowDialogue(false);

        inputProcess = new InputProcessActor(this);
        return true;
    }

    private void Update()
    {
        if(!bInitialized)
        {
            return;
        }
        if(bSkipping)
        {
            skipTimeCheck += Time.deltaTime;

            if(skipTimeCheck > 1)
            {
                EndDialogue();
            }
        }

        if (!bCodeRuning || !bAutoPass)
        {
            return;
        }

        if (ui.CheckAnimationEnd())
        {
            bCodeRuning = Process();
        }

    }

    private bool bPaused = false;
    public void PauseDialogue()
    {
        if (!bInitialized)
        {
            return;
        }
        ui.Puase();
        if (bCodeRuning)
        {
            bCodeRuning = false;
            bPaused = true;
        }
    }

    public void ResumeDialogue()
    {
        if (!bInitialized)
        {
            return;
        }
        ui.Resume();
        if (bPaused)
        {
            if (!bAutoPass)
            {
                GameManager.instance.ControlActor = inputProcess;
            }
            bCodeRuning = true;
            bPaused = false;
        }
    }

    private void Start()
    {
        GameLoader.instance?.AddSceneLoadCallback(InitializeData);
        ui = GameManager.instance?.UIMng.uiDialogue;
    }

    public void StartDialogueWithName(string name)
    {
        if(!bInitialized)
        {
            return;
        }
        for(int i = 0; i < _data.bytecodeName.Length; i++)
        {
            if(_data.bytecodeName[i] == name)
            {
                StartDialogue(i);
            }
        }
    }

    public void StartDialogue(int CodeIndex)
    {
        if (!bInitialized)
        {
            return;
        }
        if (code == null)
        {
            return;
        }
        if (CodeIndex < 0 || CodeIndex >= code.Length)
        {
            return;
        }
        if (code[CodeIndex] == null)
        {
            return;
        }
        curRuningCodeIndex = CodeIndex;
        curDialogueIndex = 0;
        bCodeRuning = true;
        PC = 0;
        bAutoPass = true;
        ui.SetShowDialogue(true);
        Process();
    }

    /// <summary>
    /// 대화시스템을 중간에 정지해야 할때 사용합니다.
    /// 정상적인 종료시에는 호출되지 않습니다.
    /// </summary>
    public void EndDialogue()
    {
        if (!bInitialized)
        {
            return;
        }
        if (!bAutoPass)
        {
            GameManager.instance?.TimeMng.ResumeTime(); 
            GameManager.instance?.ChangeCurActorToPlayer();
            bAutoPass = true;
        }
        PC = 0;
        bCodeRuning = false;
        ui.SetShowDialogue(false);
    }

    public bool Process()
    {
        if (!bInitialized)
            return false;

        bool bCycleEnd = false;

        while (!bCycleEnd)
        {
            if (PC >= code[curRuningCodeIndex].Length)
            {
                ui.SetShowDialogue(false);
                return false;
            }

            /*
             * 제공할 명령어들
             * 
             * 다음 대화 0x01 Next
             * 대화 이동(키값) 0x02 Move Key
             * 시간 정지 0x10 StopTime
             * 시간 재개 0x11 ResumeTime
             * 왼쪽 초상화 교체(스프라이트) 0x20 ChangeLeft Key
             * 오른쪽 초상화 교체(스프라이트) 0x21 ChangeRight Key
             * 왼쪽 초상화 하이라이트 0x22 HighlightLeft
             * 오른쪽 초상화 하이라이트 0x23 HighlightRight
             * 초상화 활성화 0x24 EnablePortrait
             * 초상화 비활성화 0x25 DisablePortrait
             * 대화 강조 0x03 Bold
             * 대화문 위치 위로 변경 0x04 SetUpper
             * 대화문 위치 아래로 변경 0x05 SetDown
             * 대화 화자 이름 변경 0x06 ChangeName key
             */
            string key;
            string text;
            switch (code[curRuningCodeIndex][PC])
            {
                case 0x01:
                    ui.ShowDialoge();
                    if (curDialogueIndex < 0 || curDialogueIndex >= dialogues.Length)
                    {
                        text = "Error";
                    }
                    else
                    {
                        text = dialogues[curDialogueIndex];
                    }
                    ui.ShowText(text, bAutoPass ? 1 : 0);
                    curDialogueIndex++;
                    bCycleEnd = true;
                    break;
                case 0x02:
                    key = GetStringValue();
                    if (indexKeys.ContainsKey(key))
                    {
                        curDialogueIndex = indexKeys[key];
                    }
                    break;
                case 0x03:
                    ui.SetBold();
                    break;
                case 0x04:
                    ui.SetTextPosition(true);
                    break;
                case 0x05:
                    ui.SetTextPosition(false);
                    break;
                case 0x06:
                    key = GetStringValue();
                    if(string.Compare(key, "none",true) == 0)
                    {
                        ui.SetShowName(false);
                    }
                    if (indexKeys.ContainsKey(key))
                    {
                        ui.SetShowName(true);
                        int nameIndex = indexKeys[key];
                        ui.ChangeName(dialogues[nameIndex]);
                    }
                    break;
                case 0x10:
                    ui.ShowInteractiveButton(true);
                    GameManager.instance?.TimeMng.StopTime();
                    GameManager.instance.ControlActor = inputProcess;
                    GameManager.instance?.UIMng.SetShowCommandInfo(false);
                    bAutoPass = false;
                    break;
                case 0x11:
                    ui.ShowInteractiveButton(false);
                    GameManager.instance?.TimeMng.ResumeTime();
                    GameManager.instance?.ChangeCurActorToPlayer();
                    GameManager.instance?.UIMng.SetShowCommandInfo(!GameManager.instance.SettingData.bDontUseCommandAssist);
                    bAutoPass = true;
                    break;
                case 0x20:
                    ui.SetLeftPortrait(GetStringValue());
                    break;
                case 0x21:
                    ui.SetRightPortrait(GetStringValue());
                    break;
                case 0x22:
                    ui.SetPortraitHighlight(true);
                    break;
                case 0x23:
                    ui.SetPortraitHighlight(false);
                    break;
                case 0x24:
                    ui.EnablePortrait(true);
                    break;
                case 0x25:
                    ui.EnablePortrait(false);
                    break;
            }

            PC++;
        }
        return true;
    }

    private string GetStringValue()
    {
        int length = (int)code[curRuningCodeIndex][++PC];
        byte[] byteKey = new byte[length];
        System.Array.Copy(code[curRuningCodeIndex], ++PC, byteKey, 0, length);
        PC += length - 1;

        return Encoding.Default.GetString(byteKey);
    }

    public class InputProcessActor : IActor
    {
        DialogueSystem _dialogueSystem;

        public InputProcessActor(DialogueSystem dialogueStytem)
        {
            _dialogueSystem = dialogueStytem;
        }

        public Unit GetUnit()
        {
            return null;
        }

        public bool Transition(TransitionCondition condition, object param = null)
        {
            switch (condition)
            {
                case TransitionCondition.DialogueNext:
                    if (!_dialogueSystem.ui.CheckAnimationEnd())
                    {
                        _dialogueSystem.ui.FinishAnimation();
                    }
                    else
                    {
                        _dialogueSystem.bCodeRuning = _dialogueSystem.Process();
                    }
                    break;
                case TransitionCondition.DialogueSkip:
                    _dialogueSystem.bSkipping = true;
                    break;
                case TransitionCondition.DialogueSkipCancel:
                    _dialogueSystem.bSkipping = false;
                    _dialogueSystem.skipTimeCheck = 0;
                    break;
            }

            return false;
        }
    }
}

/*
 * 제공할 명령어들
 * 
 * 다음 대화 0x01
 * 대화 이동(키값) 0x02
 * 시간 정지 0x10
 * 시간 재개 0x11
 * 왼쪽 초상화 교체(스프라이트) 0x20
 * 오른쪽 초상화 교체(스프라이트) 0x21
 * 왼쪽 초상화 하이라이트 0x22
 * 오른쪽 초상화 하이라이트 0x23
 * 초상화 활성화 0x24
 * 초상화 비활성화 0x25
 * 대화 강조 0x03
 * 대화문 위치 위로 변경 0x04
 * 대화문 위치 아래로 변경 0x05
 * 
 */