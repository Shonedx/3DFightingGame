using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandLineManager : MonoBehaviour
{
    public static CommandLineManager Instance;

    [SerializeField] private CommandLineUI commandLineUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 显示系统消息
    public static void ShowSystemMessage(string message)
    {
        if (Instance != null && Instance.commandLineUI != null)
        {
            Instance.commandLineUI.AddSystemMessage(message);
        }
    }

    // 显示用户命令反馈
    public static void ShowUserCommand(string command)
    {
        if (Instance != null && Instance.commandLineUI != null)
        {
            Instance.commandLineUI.AddUserCommand(command);
        }
    }

    // 显示错误消息
    public static void ShowErrorMessage(string message)
    {
        if (Instance != null && Instance.commandLineUI != null)
        {
            Instance.commandLineUI.AddErrorMessage(message);
        }
    }

    // 显示状态更新
    public static void ShowStatusUpdate(string update)
    {
        if (Instance != null && Instance.commandLineUI != null)
        {
            Instance.commandLineUI.ShowStatusUpdate(update);
        }
    }
}
