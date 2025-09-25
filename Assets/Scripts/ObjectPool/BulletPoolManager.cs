using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance;

    public int maxPoolSize = 200;
    public GameObject bulletPrefab; //用来实例化子弹的预制体
    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        InitializePool();
    }
    private void InitializePool()
    {
        for(int i = 0; i < maxPoolSize; i++) 
        {
        
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }
    public GameObject GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            Debug.Log("对象池对象为空，已新建一个");
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(true);
            return bullet;
        }
    }
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}
