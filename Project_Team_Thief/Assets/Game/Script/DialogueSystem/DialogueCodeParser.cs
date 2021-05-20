using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using UnityEngine;

public class DialogueCodeParser
{
    public bool Parse(in string codeString, in string fileName)
    {
        string path = Application.dataPath + "/" + fileName + ".bytes";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);

        var code = Regex.Split(codeString, "\r\r|\n|\r", RegexOptions.IgnoreCase);

        int timeStopCount = 0;

        for(int i = 0; i < code.Length; i++)
        {
            if (string.Compare(code[i], "next", true) == 0)
            {
                bw.Write((byte)0x01);
            }
            else if (Regex.IsMatch(code[i], "move \\S+", RegexOptions.IgnoreCase))
            {
                code[i] = Regex.Replace(code[i], "(^.*move )|(\\W*$)", string.Empty, RegexOptions.IgnoreCase);
                byte[] binary = Encoding.UTF8.GetBytes(code[i]);
                bw.Write((byte)0x02);
                bw.Write((byte)binary.Length);
                bw.Write(binary);
            }
            else if (string.Compare(code[i], "stoptime", true) == 0)
            {
                i++;
                timeStopCount++;
                bw.Write((byte)0x10);
            }
            else if (string.Compare(code[i], "}", true) == 0)
            {
                if (timeStopCount > 0)
                {
                    bw.Write((byte)0x11);
                    timeStopCount--;
                }
            }
        }

        return true;
    }
}
