using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Util.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "Help Command", menuName = "Utilities/DeveloperConsole/Commands/Help")]
    public class ConsoleCmdHelp : ConsoleCommand
    {
        [SerializeField, TextArea] private string HelpText = string.Empty;

        public override bool Process(out string resultMsg, string[] args)
        {
            resultMsg = HelpText;

            return true;
        }
    }
}