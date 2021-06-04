using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialogueMaker
{
    public bool MakeDialogueScript(in string buffer, out string[] Script, out Dictionary<string, int> bookmark)
    {
        Script = null;
        bookmark = null;

        var strings = Regex.Split(buffer, "\r\n|\n|\r", RegexOptions.IgnoreCase);
        // 여기 안되고 있음
        /*
         * 정규 표현식
         * () 표현식 감싸기
         * | or
         * (\r\n)|\n|\r
         * 개행문자(\r\n,\n,\r)
         */

        int length = strings.Length;
        string[] dialogues = new string[length];
        Dictionary<string, int> bookmarks = new Dictionary<string, int>();

        int index = 0;
        for (int i = 0; i < length; i++)
        {
            if (strings[i].Length == 0)
            {
                continue;
            }
            else if (Regex.IsMatch(strings[i], "^<\\w+>$"))
            {
                bookmarks.Add(Regex.Replace(strings[i], "[<>]", ""), index);
            }
            else
            {
                dialogues[index] = Regex.Replace(strings[i], "#", "\n");
                index++;
            }
        }
        System.Array.Resize<string>(ref dialogues, index);

        Script = dialogues;
        bookmark = bookmarks;

        return true;
    }
}
