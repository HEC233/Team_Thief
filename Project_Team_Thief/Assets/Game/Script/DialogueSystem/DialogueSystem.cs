using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    DialogueData _data;
    string[] dialogues;
    Dictionary<string, int> indexKeys;
    byte[] code;

    private bool autoPass = false;
    private int PC;

    private bool initialized = false;
    private bool endFlag = false;
    public bool Initialize(out string ErrorMessage)
    {
        initialized = false;
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
        TextAsset bytecode = Addressable.instance.GetText(_data.bytecodeName);
        if(text == null || bytecode == null)
        {
            ErrorMessage = "DialogueData is invalid.";
            return false;
        }
        if (!new DialogueMaker().MakeDialogueScript(text.text, out dialogues, out indexKeys))
        {
            ErrorMessage = "Failed making dailogue script.";
            return false;
        }
        code = bytecode.bytes;
        PC = 0;

        endFlag = false;
        initialized = true;
        return true;
    }

    private void Update()
    {
        
    }

    private void Start()
    {
        GameLoader.instance.AddSceneLoadCallback(Initialize);
        DontDestroyOnLoad(this.gameObject);
    }

    public bool Process()
    {
        if (!initialized)
            return false;

        if(PC >= code.Length)
        {
            endFlag = true;
            return false;
        }

        switch (code[PC])
        {
            case 0x01:
                break;
            case 0x02:
                break;
            case 0x03:
                break;
            case 0x04:
                break;
            case 0x05:
                break;
            case 0x10:
                break;
            case 0x11:
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

        return true;
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
 * 
 */