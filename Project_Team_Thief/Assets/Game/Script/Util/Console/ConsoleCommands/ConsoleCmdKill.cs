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
                IActor controlActor = gameMng.GetControlActor();
                if (controlActor != null)
                {
                    if (controlActor.Transition(TransitionCondition.ForceKill))
                    {
                        resultMsg = returnTxt;
                        return true;
                    }
                    returnTxt = "failed kill";
                }
                else
                    returnTxt = "controlling unit doesn't exist";
            }
            else
                returnTxt = "GameManager doesn't exist";

            resultMsg = returnTxt;
            return false;
        }
    }
}