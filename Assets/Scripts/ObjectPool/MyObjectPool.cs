using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObjectPool : MonoBehaviour
{
    public static MyObjectPool Instance;

    public int maxPoolSize = 100;
    public GameObject objectPrefab; //����ʵ�����ӵ���Ԥ����
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
                Debug.LogError("����Ϊ��");
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.Log("����ض���Ϊ�գ����½�һ��");
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
