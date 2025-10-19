using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
public class CommandLineUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentText; //命令台显示的文本
    [SerializeField] private ScrollRect scrollRect; // 滚动视图
    [SerializeField] private TMP_InputField commandInput; //输入框
    [SerializeField] private Button sendButton; // 发送按钮

    [SerializeField] private int maxLines = 50; // 最大显示行数
    [SerializeField] private string prompt = "> "; // 命令提示符
    [SerializeField] private Color systemColor = Color.cyan;
    [SerializeField] private Color userColor = Color.green;
    [SerializeField] private Color errorColor = Color.red;

    private List<string> messageHistory = new List<string>();
    private StringBuilder textBuilder = new StringBuilder();

    private void Awake()
    {
        // 绑定事件
        sendButton.onClick.AddListener(OnSendCommand);
        commandInput.onEndEdit.AddListener(OnInputEndEdit);

        // 初始化显示
        AddSystemMessage("command system succeed!");
        AddSystemMessage("using \"help\" to check useful commands");
        UpdateDisplay();
    }

    // 添加系统消息（如状态更新）
    public void AddSystemMessage(string message)
    {
        string timestamp = System.DateTime.Now.ToString("[HH:mm:ss] ");
        AddColoredLine(timestamp + message, systemColor);
    }

    // 添加用户命令
    public void AddUserCommand(string command)
    {
        AddColoredLine(prompt + command, userColor);
    }

    // 添加错误消息
    public void AddErrorMessage(string message)
    {
        AddColoredLine("error: " + message, errorColor);
    }

    // 添加带颜色的行
    private void AddColoredLine(string text, Color color)
    {
        // 格式化带颜色的文本
        string colorHex = ColorUtility.ToHtmlStringRGBA(color);
        string formattedLine = $"<color=#{colorHex}>{text}</color>\n";

        messageHistory.Add(formattedLine);

        // 超过最大行数则移除最旧的
        if (messageHistory.Count > maxLines)
        {
            messageHistory.RemoveAt(0);
        }

        UpdateDisplay();
    }

    // 更新显示内容
    private void UpdateDisplay()
    {
        textBuilder.Clear();

        foreach (string line in messageHistory)
        {
            textBuilder.Append(line);
        }

        contentText.text = textBuilder.ToString();

        // 自动滚动到底部
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    // 发送命令
    private void OnSendCommand()
    {
        if (!string.IsNullOrEmpty(commandInput.text))
        {
            string command = commandInput.text;
            AddUserCommand(command);

            // 处理命令（这里可以根据实际需求扩展）
            ProcessCommand(command);

            commandInput.text = "";
        }

        commandInput.ActivateInputField();
    }

    // 输入框结束编辑时（按回车）
    private void OnInputEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSendCommand();
        }
    }

    // 处理命令
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

    // 外部调用显示状态更新
    public void ShowStatusUpdate(string update)
    {
        AddSystemMessage(update);
    }
}
