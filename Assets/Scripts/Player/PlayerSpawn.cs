using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player
{
    public class PlayerSpawn : MonoBehaviour
    {
        static public PlayerSpawn Instance;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject playerPrefab;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        private void Start()
        {
            SpawnPlayer();
        }
        public void SpawnPlayer()
        {
            playerPrefab.transform.position = spawnPoint.position;
            CommandLineManager.ShowStatusUpdate("Player Spawned");
        }

    }

}
