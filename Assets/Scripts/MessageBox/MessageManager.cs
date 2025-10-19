using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance; // 单例实例

    [SerializeField] private MessageUI messagePrefab; // 消息预制体 MessageTemplate
    [SerializeField] private Transform messageParent;  // 消息容器（MessageContainer）
    [SerializeField] private float verticalSpacing = 40f; // 消息间距

    private List<MessageUI> activeMessages = new List<MessageUI>(); // 当前活跃的消息列表

    private void Awake()
    {
        // 单例模式：确保全局唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 显示新消息
    public void ShowMessage(string text, float lifeTime = 2f, float fadeTime = 0.5f)
    {
        // 实例化消息预制体
        MessageUI newMessage = Instantiate(messagePrefab, messageParent);
        newMessage.gameObject.SetActive(true); // 激活消息
        newMessage.Initialize(text, lifeTime, fadeTime);

        // 添加到列表并重新排列
        activeMessages.Add(newMessage);
        RearrangeMessages();
    }

    // 消息销毁时调用，更新列表并重新排列
    public void OnMessageDestroyed(MessageUI message)
    {
        if (activeMessages.Contains(message))
        {
            activeMessages.Remove(message);
            RearrangeMessages();
        }
    }

    // 重新排列所有消息位置（避免重叠）
    private void RearrangeMessages()
    {
        for (int i = 0; i < activeMessages.Count; i++)
        {
            RectTransform rect = activeMessages[i].GetComponent<RectTransform>();
            // 每个消息在Y轴上偏移 i * 间距（从上到下排列）
            rect.anchoredPosition = new Vector2(0, -i * verticalSpacing);
        }
    }
}
