using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    public float lifeTime = 2f;   // 显示时长
    public float fadeTime = 0.5f; // 渐变时长

    private void Awake()
    {
        // 自动获取组件（若未在Inspector手动赋值）
        if (messageText == null || canvasGroup == null)
            Debug.LogError("MessageBox相关组件未手动赋值");
    }

    // 设置消息内容并启动生命周期
    public void Initialize(string text, float life = 2f, float fade = 0.5f) //其他部分调用的时候可以该参数
    {
        messageText.text = text;
        lifeTime = life;
        fadeTime = fade;
        canvasGroup.alpha = 1f; // 确保初始可见
        StartCoroutine(FadeOut());
    }

    // 渐变消失协程
    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(lifeTime); // 等待显示时长

        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1 - (elapsed / fadeTime); // 透明度从1→0
            yield return null;
        }

        Destroy(gameObject); // 完全消失后销毁
    }

    // 消息销毁时调用（用于通知管理器重新排列）
    private void OnDestroy()
    {
        MessageManager.Instance.OnMessageDestroyed(this);
    }
}
