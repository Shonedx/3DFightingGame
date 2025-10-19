using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Health_Namespace;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Gun gun;
    [SerializeField] private Health playerHealthManager;

    [Header("�ӵ�UI����")]
    public TextMeshProUGUI bulletsText;

    private int magzineCapacity = 30;
    private int currentMagzineCapacity;

    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image Fill;

    private int maxHealth;
    private void Awake()
    {
        magzineCapacity = gun.GetMagzineCap();
        slider.maxValue = 1;
        slider.value = slider.maxValue;
        maxHealth = playerHealthManager.GetMaxHealth();
        playerHealthManager.OnHealthChanged += SetHealthBar;
    }
    private void Update()
    {
        UpdateBulletUI();
    }
    public void SetHealthBar(int currentHealth) //��OnHealthChanged�е���
    {
        slider.value = (float)currentHealth /(float)maxHealth;
        Fill.color=gradient.Evaluate(slider.normalizedValue); //ʹ��Ѫ������
    }
    public void UpdateBulletUI()
    {
        currentMagzineCapacity = gun.GetCurrentMagzineCap();
        bulletsText.text =$"{currentMagzineCapacity}/{magzineCapacity}"; 
    }
    private void OnDestroy()
    {
        playerHealthManager.OnHealthChanged -= SetHealthBar;
    }
}
