using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace PS.Util.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "Spawn Command", menuName = "Utilities/DeveloperConsole/Commands/Spawn")]
    public class ConsoleCmdSpawn : ConsoleCommand
    {
        public override bool Process(out string resultMsg, string[] args)
        {
            string returnTxt = string.Empty;

            if (args.Length > 0)
            {
                Assert.IsNotNull(GameManager.instance.spawner);

                if (!GameManager.instance.spawner.Spawn(args[0], GameManager.instance.GetControlActor().GetUnit().transform.position))
                {
                    resultMsg = returnTxt;
                    return true;
                }
                returnTxt = "There is no such unit";
            }
            else
            {
                returnTxt = "invalid command";
            }

            resultMsg = returnTxt;
            return false;
        }
    }
}
