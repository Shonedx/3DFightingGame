using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    public class EnemySpawn : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private int maxEnemies = 5;

        private int activeEnemyCount = 0;
        private void Start()
        {
            InitializeEnemies();
        }
        private void Update()
        {
            if(activeEnemyCount <= 0)
            {
                SpawnEnemy();
            }
        }
        private void InitializeEnemies()
        {
            while (activeEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
        private void SpawnEnemy()
        {
            if (activeEnemyCount < maxEnemies)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
                activeEnemyCount++;
            }
        }
        public void DeleteEnemy()
        {
            if (activeEnemyCount > 0)
            {
                activeEnemyCount--;
                CommandLineManager.ShowStatusUpdate($"Enemy defeated! Remaining enemies: {activeEnemyCount}");
            }
        }
        public int GetEnemyCount() => activeEnemyCount;


    }

}
