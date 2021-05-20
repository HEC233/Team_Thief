using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public DialogueUIController ui;

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
            ErrorMessage = "DialogueData is invalid.";
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
                ErrorMessage = "DialogueData is invalid.";
                return false;
            }
            code[i] = bytecode.bytes;
        }
        PC = 0;
        bInitialized = true;
        bCodeRuning = false;

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
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartDialogue(int CodeIndex)
    {
        if (CodeIndex < 0 || CodeIndex >= code.Length || code[CodeIndex] == null)
            return;
        curRuningCodeIndex = CodeIndex;
        curDialogueIndex = 0;
        bCodeRuning = true;
        PC = 0;
        bAutoPass = true;
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
             * 대화 강조 0x03 
             * 대화문 위치 위로 변경 0x04
             * 대화문 위치 아래로 변경 0x05
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
                    ui.ShowText(text);
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
                    break;
                case 0x04:
                    break;
                case 0x05:
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

                    break;
                case 0x21:
                    break;
                case 0x22:
                    break;
                case 0x23:
                    break;
                case 0x24:
                    break;
                case 0x25:
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