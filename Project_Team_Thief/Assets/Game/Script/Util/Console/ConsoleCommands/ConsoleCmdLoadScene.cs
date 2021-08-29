using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Util.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "Help Command", menuName = "Utilities/DeveloperConsole/Commands/LoadScene")]
    public class ConsoleCmdLoadScene : ConsoleCommand
    {
        public override bool Process(out string resultMsg, string[] args)
        {
            GameManager.instance.LoadScene(args[0]);

            resultMsg = string.Empty;

            return true;
        }
    }
}
