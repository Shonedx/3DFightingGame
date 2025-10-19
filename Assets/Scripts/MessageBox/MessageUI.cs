using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    public float lifeTime = 2f;   // ��ʾʱ��
    public float fadeTime = 0.5f; // ����ʱ��

    private void Awake()
    {
        // �Զ���ȡ�������δ��Inspector�ֶ���ֵ��
        if (messageText == null || canvasGroup == null)
            Debug.LogError("MessageBox������δ�ֶ���ֵ");
    }

    // ������Ϣ���ݲ�������������
    public void Initialize(string text, float life = 2f, float fade = 0.5f) //�������ֵ��õ�ʱ����Ըò���
    {
        messageText.text = text;
        lifeTime = life;
        fadeTime = fade;
        canvasGroup.alpha = 1f; // ȷ����ʼ�ɼ�
        StartCoroutine(FadeOut());
    }

    // ������ʧЭ��
    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(lifeTime); // �ȴ���ʾʱ��

        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1 - (elapsed / fadeTime); // ͸���ȴ�1��0
            yield return null;
        }

        Destroy(gameObject); // ��ȫ��ʧ������
    }

    // ��Ϣ����ʱ���ã�����֪ͨ�������������У�
    private void OnDestroy()
    {
        MessageManager.Instance.OnMessageDestroyed(this);
    }
}
