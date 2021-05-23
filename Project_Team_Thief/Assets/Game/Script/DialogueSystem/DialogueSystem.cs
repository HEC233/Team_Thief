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

    public bool CheckRunning() { return bCodeRuning; }


    InputProcessActor inputProcess;
    IActor player = null;

    public bool InitializeData(out string ErrorMessage)
    {
        bInitialized = false;
        ErrorMessage = string.Empty;

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

        ui = GameManager.instance?.uiMng.uiDialogue;

        inputProcess = new InputProcessActor(this);
        return true;
    }

    private void Update()
    {
        if (!bCodeRuning || !bAutoPass)
        {
            return;
        }

        if (ui.CheckAnimationEnd())
        {
            bCodeRuning = Process();
        }
    }

    private void Start()
    {
        GameLoader.instance.AddSceneLoadCallback(InitializeData);
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
    }

    public void EndDialogue()
    {
        if(!bAutoPass)
        {
            GameManager.instance?.timeMng.ResumeTime();
            GameManager.instance?.SetControlUnit(player);
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
             */
            switch (code[curRuningCodeIndex][PC])
            {
                case 0x01:
                    string text;
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
                    string key = GetStringValue();
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
                case 0x10:
                    GameManager.instance?.timeMng.StopTime();
                    player = GameManager.instance?.GetControlActor();
                    GameManager.instance?.SetControlUnit(inputProcess);
                    bAutoPass = false;
                    break;
                case 0x11:
                    GameManager.instance?.timeMng.ResumeTime();
                    GameManager.instance?.SetControlUnit(player);
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
                case TransitionCondition.Attack:
                    if (!_dialogueSystem.ui.CheckAnimationEnd())
                    {
                        _dialogueSystem.ui.FinishAnimation();
                    }
                    else
                    {
                        _dialogueSystem.bCodeRuning = _dialogueSystem.Process();
                    }
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