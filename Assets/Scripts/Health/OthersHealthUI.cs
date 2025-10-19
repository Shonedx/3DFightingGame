using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Health_Namespace
{
    public class OthersHealthUI : MonoBehaviour
    {
        //�ⲿ�ű�
        [SerializeField] private Health healthManager;

        [SerializeField] private Slider slider;
        [SerializeField] private Gradient gradient;
        [SerializeField] private Image Fill;
        private int maxHealth;
        private void Awake()
        {
            healthManager.OnHealthChanged += SetHealthBar;
            maxHealth= healthManager.GetMaxHealth();
            slider.maxValue = 1;
            slider.value = slider.maxValue;
        }
        public void SetHealthBar(int currentHealth) //Ѫ���仯��UI�仯
        {
            slider.value = (float)currentHealth / (float)maxHealth;
            Fill.color = gradient.Evaluate(slider.normalizedValue); //ʹ��Ѫ����ɫ����
        }
        private void OnDestroy()
        {
            healthManager.OnHealthChanged -= SetHealthBar;
        }
    }
}
