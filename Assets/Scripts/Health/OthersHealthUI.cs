using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Health_Namespace
{
    public class OthersHealthUI : MonoBehaviour
    {
        //外部脚本
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
        public void SetHealthBar(int currentHealth) //血量变化是UI变化
        {
            slider.value = (float)currentHealth / (float)maxHealth;
            Fill.color = gradient.Evaluate(slider.normalizedValue); //使得血量颜色渐变
        }
        private void OnDestroy()
        {
            healthManager.OnHealthChanged -= SetHealthBar;
        }
    }
}
