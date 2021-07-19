using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

// https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/#comment-7111
public class CSVReader : MonoBehaviour
{
    // Start is called before the first frame update
    
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; // 정규 표현식
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static char[] TRIM_CHARS = {'\"'};
    
    public static List<Dictionary<string, object>> Read(string fileName)
    {
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        TextAsset csvData = Addressable.instance.GetText(fileName);

        var lines = Regex.Split(csvData.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
        {
            return list;
        }
        
        var header = Regex.Split(lines[0], SPLIT_RE);

        for (int i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
            {
                continue;
            }

            var entry = new Dictionary<string,  object>();

            for (int j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalValue = value;
                
                int n;
                float f;

                if (int.TryParse(value, out n))
                {
                    finalValue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalValue = f;
                }

                entry[header[j]] = finalValue;
            }
            
            list.Add(entry);
        }
           

        return list;
    }
    
}
