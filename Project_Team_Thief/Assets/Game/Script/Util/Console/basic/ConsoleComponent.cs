using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PS.Util.DeveloperConsole.Commands;

namespace PS.Util.DeveloperConsole
{
    public class ConsoleComponent : MonoBehaviour
    {
        [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas = null;
        [SerializeField] private TMP_InputField inputField = null;
        [SerializeField] private TMP_Text textField = null;

        private static DeveloperConsole developerConsole;
        private DeveloperConsole DevConsole
        {
            get
            {
                if (developerConsole == null) developerConsole = new DeveloperConsole(commands);
                return developerConsole = new DeveloperConsole(commands);
            }
        }
        private float previousTimeScale = 1.0f;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            if (developerConsole == null)
                developerConsole = new DeveloperConsole(commands);
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.BackQuote))
            //    ToggleUi();

        }

        public void ToggleUi()
        {
            if (uiCanvas.activeSelf)
            {
                inputField.text = string.Empty;
                uiCanvas.SetActive(false);
                Time.timeScale = previousTimeScale;
            }
            else
            {
                uiCanvas.SetActive(true);
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0;
                inputField.ActivateInputField();
            }
        }

        public void ProcessCommand(string input)
        {
            if(input.Length == 0)
            {
                return;
            }
            if (input.Equals("clear", System.StringComparison.OrdinalIgnoreCase))
            {
                textField.text = string.Empty;
                inputField.text = string.Empty;
                return;
            }

            textField.text = textField.text + '\n' + input;
            string result = DevConsole.ProcessCommand(input);

            if(result.Length != 0)
            {
                textField.text = textField.text + "\n@ " + result;
            }

            inputField.text = string.Empty;
        }
    }
}