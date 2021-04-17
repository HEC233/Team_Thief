using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Util.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "Spawn Command", menuName = "Utilities/DeveloperConsole/Commands/Spawn")]
    public class ConsoleCmdSpawn : ConsoleCommand
    {
        public List<GameObject> unit = new List<GameObject>();

        public override bool Process(out string resultMsg, string[] args)
        {
            string returnTxt = string.Empty;

            if (args.Length > 0)
            {
                for (int i = 0; i < unit.Count; i++)
                {
                    var unitComponent = unit[i].GetComponentInChildren<Unit>();
                    if (unitComponent && unitComponent.GetUnitName() == args[0])
                    {
                        Instantiate(unit[i], GameManager.instance.GetControlActor().GetUnit().transform.position, Quaternion.identity);
                        resultMsg = returnTxt;
                        return true;
                    }
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
