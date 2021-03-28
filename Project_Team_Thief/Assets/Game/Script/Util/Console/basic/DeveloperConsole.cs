using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PS.Util.DeveloperConsole.Commands;

namespace PS.Util.DeveloperConsole
{
    public class DeveloperConsole
    {
        private readonly IEnumerable<ConsoleCommand> commands;

        public DeveloperConsole(IEnumerable<ConsoleCommand> commands)
        {
            this.commands = commands;
        }

        public string ProcessCommand(string inputString)
        {
            string[] splitString = inputString.Split(' ');

            string commandName = splitString[0];
            string[] args = splitString.Skip(1).ToArray();

            return ProcessCommand(commandName, args);
        }

        public string ProcessCommand(string commandName, string[]args)
        {
            string result = string.Empty;

            foreach(var c in commands)
            {
                if (!commandName.Equals(c.CommandName, System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if(!c.Process(out result, args))
                    result = "Command Failed : " + result;

                return result;
            }

            result = "there's no command";
            return result;
        }
    }
}
