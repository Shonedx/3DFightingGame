using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    
    private float lifeTime = 50f;
    private float delayTime = .5f;
    [SerializeField]
    private float hitForce=10f;
    void Start()
    {
        StartCoroutine(AutoRecycle(lifeTime));
    }
    IEnumerator AutoRecycle(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
        BulletPoolManager.Instance.ReturnBullet(this.gameObject); //»ØÊÕ×Óµ¯
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.rigidbody!=null)
        collision.rigidbody.AddForce(this.transform.forward*hitForce, ForceMode.Impulse);
        Debug.Log("collision:" + collision.gameObject.name);
        StartCoroutine(AutoRecycle(delayTime));        
    }
}
