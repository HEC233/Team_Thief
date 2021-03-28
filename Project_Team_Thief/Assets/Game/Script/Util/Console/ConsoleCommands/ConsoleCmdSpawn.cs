using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Util.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "Spawn Command", menuName = "Utilities/DeveloperConsole/Commands/Spawn")]
    public class ConsoleCmdSpawn : ConsoleCommand
    {
        public Dictionary<string, GameObject> units;

        public override bool Process(out string resultMsg, string[] args)
        {
            string returnTxt = string.Empty;
            resultMsg = returnTxt;
            return false;
        }
    }
}
