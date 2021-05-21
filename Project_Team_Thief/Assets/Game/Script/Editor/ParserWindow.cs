using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParserWindow : EditorWindow
{
    private TextAsset code;

    [MenuItem("Window/DialogueCode Parser")]
    public static void Init()
    {
        ParserWindow window = (ParserWindow)EditorWindow.GetWindow(typeof(ParserWindow));
        window.Show();
    }

    private void OnGUI()
    {
        code = (TextAsset)EditorGUILayout.ObjectField("code", code, typeof(TextAsset), false);

        if(GUILayout.Button("Parse"))
        {
            if (code != null)
                Parse();
        }
    }

    private void Parse()
    {
        if (!new DialogueCodeParser().Parse(code.text, code.name))
            Debug.LogError("Parsing failed!");
    }
}
