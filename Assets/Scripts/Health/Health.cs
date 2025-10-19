using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Health_Namespace
{
    public class Health : MonoBehaviour
    {
        public delegate void HealthChangeHandler(int currentHealth);
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int health; //currentHealth
        [SerializeField] private int minHealth = 0;
        [Header("事件响应")]
        [Tooltip("当血量变化时触发")]
        public HealthChangeHandler OnHealthChanged;
        private void Start()
        {
            SetHealthToMax();
        }
        public void TakeDamage(int damage)
        {
            MinusHealth(damage);
            if (health <= 0)
                Die();
        }
        public void Die()
        {
            if (this.gameObject.CompareTag("Enemy"))
            {
                FindObjectOfType<Enemy.EnemySpawn>()?.DeleteEnemy();
                Destroy(this.gameObject);
            }
            else if (this.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Died");
                PlayerSpawn.Instance?.SpawnPlayer(); //玩家死亡重生
            }
        }
        public void AddHealth(int delta)
        {
            health = Math.Min(maxHealth, health + delta);
            OnHealthChanged?.Invoke(health);
        }
        public void MinusHealth(int delta)
        {
            health = Math.Max(minHealth, health - delta);
            OnHealthChanged?.Invoke(health);
        }
        public void SetHealthToMax()
        {
            health = maxHealth;
        }
        public int GetHealth() => (health > maxHealth) ? maxHealth : (health < minHealth) ? minHealth : health;
        public int GetMaxHealth() => maxHealth;
    }
}

