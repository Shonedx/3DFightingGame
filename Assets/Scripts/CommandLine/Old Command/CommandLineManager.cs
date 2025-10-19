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

    // ��ʾϵͳ��Ϣ
    public static void ShowSystemMessage(string message)
    {
        if (Instance != null && Instance.commandLineUI != null)
        {
            Instance.commandLineUI.AddSystemMessage(message);
        }
    }

    // ��ʾ�û������
    public static void ShowUserCommand(string command)
    {
        if (Instance != null && Instance.commandLineUI != null)
        {
            Instance.commandLineUI.AddUserCommand(command);
        }
    }

    // ��ʾ������Ϣ
    public static void ShowErrorMessage(string message)
    {
        if (Instance != null && Instance.commandLineUI != null)
        {
            Instance.commandLineUI.AddErrorMessage(message);
        }
    }

    // ��ʾ״̬����
    public static void ShowStatusUpdate(string update)
    {
        if (Instance != null && Instance.commandLineUI != null)
        {
            Instance.commandLineUI.ShowStatusUpdate(update);
        }
    }
}
