using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DebugConsole : Singleton<DebugConsole>
{
    [SerializeField] private Canvas consoleUI;
    [SerializeField] private ScrollRect consoleScrollRect;
    [SerializeField] private Text textField;
    [SerializeField] private InputField inputField;

    private static Dictionary<string, Action> commands;

    private IEnumerable<string> SceneNames
    {
        get => Enumerable.Range(0, SceneManager.sceneCountInBuildSettings).Select(i => SceneUtility.GetScenePathByBuildIndex(i)).Select(p => Path.GetFileNameWithoutExtension(p));
    }

    private float commandInputDelay = 0.1f;
    private float lastCommandInputTime;

    private void Awake()
    {
        if (!this.CheckSingletonInstance(true))
            return;

        if (commands == null)
            InstantiateCommand();

        InitializeConsole();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Slash))
        {
            var playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput != null)
                playerInput.enabled = consoleUI.gameObject.activeSelf;
            consoleUI.gameObject.SetActive(!consoleUI.gameObject.activeSelf);
            inputField.ActivateInputField();
        }

        //Debug.Log(consoleScrollRect.verticalNormalizedPosition);
    }

    /// <summary>
    /// Command 입력 시 처리되는 메서드
    /// </summary>
    public void InputCommand()
    {
        if (string.IsNullOrEmpty(inputField.text))
            return;

        if (Time.time < lastCommandInputTime + commandInputDelay)
            return;

        string command = inputField.text.Trim();
        inputField.text = "";
        textField.text += $">> {command}\n";

        int idx = command.IndexOf(' ');
        if (idx >= 0)
        {
            string temp = command.Substring(0, idx);
            this.subCommand = command.Length > idx ? command.Substring(idx).Trim() : null;
            Debug.Log($"{temp} | {subCommand}");
            command = temp;
        }

        if (commands.TryGetValue(command, out var action))
        {
            action?.Invoke();
        }
        else
        {
            textField.text += $"There's no \"{command}\" command.\n\n";
        }

        StartCoroutine(RefreshTextField());
    }

    private string subCommand;

    private IEnumerator RefreshTextField()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)consoleScrollRect.transform);
        consoleScrollRect.verticalNormalizedPosition = 0f;
        inputField.ActivateInputField();
    }

    private void InstantiateCommand()
    {
        commands = new Dictionary<string, Action>()
        {
            { "clear", () => textField.text = "" },
            { "scene", SceneCommand },
        };
    }

    private void InitializeConsole()
    {
        consoleUI.gameObject.SetActive(false);
    }

    private void PrintSceneInfo()
    {
        int i = 0;
        textField.text += "=== Scene Info ===\n";
        foreach (string sceneName in this.SceneNames)
            textField.text += $"Name: {sceneName}, Index: {i++}\n";
        textField.text += "\n";
    }

    private void SceneCommand()
    {
        if (string.IsNullOrEmpty(this.subCommand))
            return;

        string subCommand = this.subCommand;
        int subCommandIdx = subCommand.IndexOf(' ');
        if (subCommandIdx >= 0)
        {
            string temp = subCommand.Substring(0, subCommandIdx);
            List<string> options = new List<string>();
            for (int i = 0; i < subCommand.Length;)
            {
                int idx = subCommand.IndexOf("--", i);
                if (idx >= 0)
                {
                    int o = subCommand.IndexOf(' ', idx);
                    options.Add(subCommand.Substring(idx, (o < 0 ? subCommand.Length : o) - idx));
                    i = idx + 1;
                }
                else
                {
                    break;
                }
            }

            subCommand = temp;
            SceneSubCommand(subCommand, options);
        }
        else
        {
            SceneSubCommand(subCommand);
        }
    }

    private void SceneSubCommand(string subCommand, List<string> options = null)
    {
        switch (subCommand)
        {
            case "info":
                this.PrintSceneInfo();
                break;
            case "load":
                try
                {
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        textField.text += "Don't call the scene load command in Start Scene.\n\n";
                    }
                    else
                    {
                        var splits = options[0].Split('=');
                        if (splits[0] == "--index")
                        {
                            int sceneIndex = int.Parse(splits[1]);
                            var player = GameObject.FindGameObjectWithTag("Player");
                            if (player != null)
                                GameManager.Instance.LoadScene(sceneIndex, player);
                            else
                                GameManager.Instance.LoadScene(sceneIndex);
                        }
                    }
                }
                catch (Exception)
                {
                    textField.text += "The scene load command error\n\n";
                }
                break;
            default:
                textField.text += $"There's no \"{subCommand}\" sub command.\n\n";
                break;
        }
    }
}