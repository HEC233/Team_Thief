using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Util.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "Log Command", menuName = "Utilities/DeveloperConsole/Commands/Log")]
    public class ConsoleCmdLog : ConsoleCommand
    {

        public override bool Process(out string resultMsg, string[] args)
        {
            string log = string.Join(" ", args);

            Debug.Log(log);

            resultMsg = string.Empty;
            return true;
        }
    }
}
