using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Util.DeveloperConsole.Commands
{
    public abstract class ConsoleCommand : ScriptableObject
    {
        [SerializeField] private string command = string.Empty;
        public string CommandName { get { return command; } }

        public abstract bool Process(out string resultMsg, string[] args);
    }
}
