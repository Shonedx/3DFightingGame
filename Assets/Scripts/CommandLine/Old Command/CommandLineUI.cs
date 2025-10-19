using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
public class CommandLineUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentText; //����̨��ʾ���ı�
    [SerializeField] private ScrollRect scrollRect; // ������ͼ
    [SerializeField] private TMP_InputField commandInput; //�����
    [SerializeField] private Button sendButton; // ���Ͱ�ť

    [SerializeField] private int maxLines = 50; // �����ʾ����
    [SerializeField] private string prompt = "> "; // ������ʾ��
    [SerializeField] private Color systemColor = Color.cyan;
    [SerializeField] private Color userColor = Color.green;
    [SerializeField] private Color errorColor = Color.red;

    private List<string> messageHistory = new List<string>();
    private StringBuilder textBuilder = new StringBuilder();

    private void Awake()
    {
        // ���¼�
        sendButton.onClick.AddListener(OnSendCommand);
        commandInput.onEndEdit.AddListener(OnInputEndEdit);

        // ��ʼ����ʾ
        AddSystemMessage("command system succeed!");
        AddSystemMessage("using \"help\" to check useful commands");
        UpdateDisplay();
    }

    // ���ϵͳ��Ϣ����״̬���£�
    public void AddSystemMessage(string message)
    {
        string timestamp = System.DateTime.Now.ToString("[HH:mm:ss] ");
        AddColoredLine(timestamp + message, systemColor);
    }

    // ����û�����
    public void AddUserCommand(string command)
    {
        AddColoredLine(prompt + command, userColor);
    }

    // ��Ӵ�����Ϣ
    public void AddErrorMessage(string message)
    {
        AddColoredLine("error: " + message, errorColor);
    }

    // ��Ӵ���ɫ����
    private void AddColoredLine(string text, Color color)
    {
        // ��ʽ������ɫ���ı�
        string colorHex = ColorUtility.ToHtmlStringRGBA(color);
        string formattedLine = $"<color=#{colorHex}>{text}</color>\n";

        messageHistory.Add(formattedLine);

        // ��������������Ƴ���ɵ�
        if (messageHistory.Count > maxLines)
        {
            messageHistory.RemoveAt(0);
        }

        UpdateDisplay();
    }

    // ������ʾ����
    private void UpdateDisplay()
    {
        textBuilder.Clear();

        foreach (string line in messageHistory)
        {
            textBuilder.Append(line);
        }

        contentText.text = textBuilder.ToString();

        // �Զ��������ײ�
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    // ��������
    private void OnSendCommand()
    {
        if (!string.IsNullOrEmpty(commandInput.text))
        {
            string command = commandInput.text;
            AddUserCommand(command);

            // �������������Ը���ʵ��������չ��
            ProcessCommand(command);

            commandInput.text = "";
        }

        commandInput.ActivateInputField();
    }

    // ���������༭ʱ�����س���
    private void OnInputEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSendCommand();
        }
    }

    // ��������
    private void ProcessCommand(string command)
    {
        command = command.ToLower();

        switch (command)
        {
            case "help":
                AddSystemMessage("useful commands:");
                AddSystemMessage("- help: show help information");
                AddSystemMessage("- clear: clear the screen");
                AddSystemMessage("- time: show current time");
                break;

            case "clear":
                messageHistory.Clear();
                AddSystemMessage("already cleared");
                break;

            case "time":
                AddSystemMessage("current time: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                break;

            default:
                AddErrorMessage("unknown: " + command + " - input \"help\" to check useful commands");
                break;
        }
    }

    // �ⲿ������ʾ״̬����
    public void ShowStatusUpdate(string update)
    {
        AddSystemMessage(update);
    }
}
