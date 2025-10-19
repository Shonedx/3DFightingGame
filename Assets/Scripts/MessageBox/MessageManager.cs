using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance; // ����ʵ��

    [SerializeField] private MessageUI messagePrefab; // ��ϢԤ���� MessageTemplate
    [SerializeField] private Transform messageParent;  // ��Ϣ������MessageContainer��
    [SerializeField] private float verticalSpacing = 40f; // ��Ϣ���

    private List<MessageUI> activeMessages = new List<MessageUI>(); // ��ǰ��Ծ����Ϣ�б�

    private void Awake()
    {
        // ����ģʽ��ȷ��ȫ��Ψһ
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �л�����������
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ��ʾ����Ϣ
    public void ShowMessage(string text, float lifeTime = 2f, float fadeTime = 0.5f)
    {
        // ʵ������ϢԤ����
        MessageUI newMessage = Instantiate(messagePrefab, messageParent);
        newMessage.gameObject.SetActive(true); // ������Ϣ
        newMessage.Initialize(text, lifeTime, fadeTime);

        // ��ӵ��б���������
        activeMessages.Add(newMessage);
        RearrangeMessages();
    }

    // ��Ϣ����ʱ���ã������б���������
    public void OnMessageDestroyed(MessageUI message)
    {
        if (activeMessages.Contains(message))
        {
            activeMessages.Remove(message);
            RearrangeMessages();
        }
    }

    // ��������������Ϣλ�ã������ص���
    private void RearrangeMessages()
    {
        for (int i = 0; i < activeMessages.Count; i++)
        {
            RectTransform rect = activeMessages[i].GetComponent<RectTransform>();
            // ÿ����Ϣ��Y����ƫ�� i * ��ࣨ���ϵ������У�
            rect.anchoredPosition = new Vector2(0, -i * verticalSpacing);
        }
    }
}
