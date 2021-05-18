using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class DialogueSystem : MonoBehaviour
{
    DialogueData _data;
    public string[] dialogues;
    Dictionary<string, int> indexKeys;

    public TextAsset asset;

    private bool initialized = false;
    public void Initialize()
    {
        var dataObject = GameObject.Find("DialogueData");
        if (dataObject == null)
            return;

        var data = dataObject.GetComponent<DialogueData>();
        if (data == null)
            return;

        _data = data;

        if (!MakeDialogue())
            return;

        

        initialized = true;
    }

    private bool MakeDialogue()
    {
        //TextAsset text = Addressable.instance.GetText(_data.dialogueName);
        TextAsset text = asset;

        if (text == null) return false;

        string buffer = text.text;

        var strings = Regex.Split(buffer, "(\\r\\n){2,}|\\n{2,}|\\r{2,}");
        // 여기 안되고 있음
        /*
         * 정규 표현식
         * () 표현식 감싸기
         * {2,} 최소한 두개이상
         * | or
         * (\r\n){2,}|\n{2,}|\r{2,}
         * 개행문자(\r\n,\n,\r) 이후 두글자 이상 있는 문자들
         */

        int length = strings.Length;
        dialogues = new string[length];

        int index = 0;
        for(int i =0;i< length;i++)
        {
            if(Regex.IsMatch(strings[i], "^<\\w+>$"))
            {
                Regex.Replace(strings[i], "[<>]", "");
                indexKeys.Add(strings[i], index);
            }
            else
            {
                dialogues[index] = strings[i];
                index++;
            }
        }
        System.Array.Resize<string>(ref dialogues, index + 1);

        return true;
    }

    private void Update()
    {
        
    }

    private void Start()
    {
        Initialize();
    }

    public bool Process()
    {
        if (!initialized)
            return false;

        return false;
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