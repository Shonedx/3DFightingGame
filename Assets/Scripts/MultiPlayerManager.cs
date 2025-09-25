using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player1;
    private Camera player1Camera;
    [SerializeField]
    private Transform player1SpawnPosition;
    [SerializeField]
    private GameObject player2;
    private Camera player2Camera;
    [SerializeField]
    private Transform player2SpawnPosition;
    
    [SerializeField]
    private bool EnableMultiPlayer = false;

    private void Awake()
    {
     
    }
    void Start()
    {
        player1Camera = player1.GetComponentInChildren<Camera>();
        if (player1Camera != null)
        {
            player1Camera.rect = new Rect(0, 0, 1, 1);
            // 设置玩家 1 的准星在左半屏中心
        }
        player1.transform.position= player1SpawnPosition.position;
        if (EnableMultiPlayer)
        {
            player2Camera = player2.GetComponentInChildren<Camera>();
            if (player2Camera != null)
            {
                player1Camera.rect = new Rect(0, 0, 0.5f, 1);
                player2Camera.rect = new Rect(0.5f, 0, 0.5f, 1);
            }
            player2.transform.position = player2SpawnPosition.position;
        }
      
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
