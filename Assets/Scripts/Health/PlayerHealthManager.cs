using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Health_Namespace
{
    public class PlayerHealthManager : MonoBehaviour
    {
        private List<Health> allHealthComponents = new List<Health>();
        public void RegisterHealth(Health health)
        {
            if (!allHealthComponents.Contains(health))
                allHealthComponents.Add(health);
        }
        public void UnregisterHealth(Health health)
        {
            allHealthComponents.Remove(health);
        }

        // 对所有对象造成伤害
        public void DealAreaDamage(int damage)
        {
            foreach (var health in allHealthComponents)
            {
                health.TakeDamage(damage);
            }
        }
    }
}

