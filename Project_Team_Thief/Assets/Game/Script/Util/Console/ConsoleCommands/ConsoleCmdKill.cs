using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Util.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "Kill Command", menuName = "Utilities/DeveloperConsole/Commands/Kill")]
    public class ConsoleCmdKill : ConsoleCommand
    {
        public override bool Process(out string resultMsg, string[] args)
        {
            string returnTxt = string.Empty;
            var gameMng = GameManager.instance;

            if (gameMng != null)
            {
                IActor controlUnit = gameMng.GetControlUnit();
                if(controlUnit != null)
                {
                    if (controlUnit.Transition(TransitionCondition.ForceKill))
                    {
                        resultMsg = returnTxt;
                        return true;
                    }
                    resultMsg = "failed kill";
                }
                resultMsg = "controlling unit doesn't exist";
            }
            resultMsg = "GameManager doesn't exist";

            resultMsg = returnTxt;
            return false;
        }
    }
}