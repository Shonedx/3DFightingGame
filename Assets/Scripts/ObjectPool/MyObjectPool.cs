using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObjectPool : MonoBehaviour
{
    public static MyObjectPool Instance;

    public int maxPoolSize = 100;
    public GameObject objectPrefab; //用来实例化子弹的预制体
    private Queue<GameObject> objectPool = new Queue<GameObject>();
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
        
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }
    public GameObject GetObject()
    {
        if (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            if (obj == null)
                Debug.LogError("对象为空");
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.Log("对象池对象为空，已新建一个");
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(true);
            return obj;
        }
    }
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }
}
