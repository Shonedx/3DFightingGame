using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class Gun : MonoBehaviour
{

    [SerializeField] private ParticleSystem metalImpactSystem;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TrailRenderer bulletsTrail; //子弹轨迹
    [SerializeField] private ParticleSystem shootingSystem;
  
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [Header("射击参数")]
    [SerializeField] private float shootingMaxDistance;//射击距离
    [SerializeField] private bool shootingSpread; //是否开启射击偏移
    [SerializeField] private Vector3 shootingSpreadVariance = new Vector3(.1f, .1f, .1f); //射击偏移值
    [SerializeField] private LayerMask Mask; //射线检测层 目前默认为everything
    [SerializeField] private float lastShootTime; //上次射击时间 用于自动开火
    [SerializeField] private float shootCooldown;  //自动开火冷却时间 指每次开火间隔 
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private int magzineCapacity=30; //弹夹容量
    [SerializeField] private int currentMagzineCapacity; //当前弹夹容量
    [SerializeField] private bool isMagzineNull=false; //弹夹是否为空
    //连发参数
    [SerializeField] public bool isAuto = false; //标志自动射击和连发模式
    [SerializeField] private int burstCount = 3; //连发次数
    [SerializeField] private int currentBurstCount = 0; //当前连发次数
    [SerializeField] private float shootingGaptime = .2f;//连发间隔时间
    private bool readyToShoot = true; //是否准备好射击
    
    //子弹轨迹运动时间
    private float trailTime;
    private void Awake()
    {
        currentMagzineCapacity = magzineCapacity;
    }
    private void Update()
    {
        isMagzineNull = currentMagzineCapacity<0;
    }
    public void ShootHandler()
    {
        if (currentMagzineCapacity > 0)
            isMagzineNull = false;
        else
        {
            currentMagzineCapacity = 0;
            isMagzineNull = true;
        }
        if (!isMagzineNull)
        {
            if (isAuto && (lastShootTime + shootCooldown < Time.time))
            {
                Shoot();
                lastShootTime = Time.time; //更新时间
            }
            else if (!isAuto && readyToShoot) //连发模式 
            {
                currentBurstCount++;
                Shoot();
                readyToShoot = false; //禁止下一次射击
                StartCoroutine(ResetShot());
            }
        }
        else 
        {
            Invoke(nameof(Reload),reloadTime) ;
        }
        currentMagzineCapacity--;
    }
    public void Reload()//上子弹
    {
        currentMagzineCapacity = magzineCapacity;
        isMagzineNull = false; //弹夹不为空
        Debug.Log("ReloadDone");

    }
    private IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(shootingGaptime);
        while (currentBurstCount != burstCount)
        {
            currentBurstCount++;
            Shoot();
            yield return new WaitForSeconds(shootingGaptime);
        }
        readyToShoot = true;
        currentBurstCount = 0;
        Debug.Log("burstDone");
    }
    public void Shoot()
    {
        Instantiate(shootingSystem, spawnPoint.position, Quaternion.LookRotation(spawnPoint.forward));
        //shootingSystem.Play();
        Vector3 targetPosition;
        Vector3 direction = GetShootDirection(out targetPosition);
        if (Physics.Raycast(spawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
        {
            GameObject trailObj = MyObjectPool.Instance.GetObject();
            TrailRenderer trail=trailObj.GetComponent<TrailRenderer>();
            trail.transform.position = spawnPoint.position;
            //TrailRenderer trail = Instantiate(bulletsTrail, spawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
        }
        else
        {
            GameObject trailObj = MyObjectPool.Instance.GetObject();
            TrailRenderer trail = trailObj.GetComponent<TrailRenderer>();
            trail.transform.position = spawnPoint.position;
            //TrailRenderer trail = Instantiate(bulletsTrail, spawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrailToMaxDistance(trail, targetPosition));

        }
    }
    
    private Vector3 GetShootDirection(out Vector3 targetPosition)
    {
        Camera currentCamera = cinemachineBrain.OutputCamera;
        Ray ray = currentCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        targetPosition = ray.GetPoint(shootingMaxDistance);
        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
        }
        Vector3 dir =targetPosition- spawnPoint.position;
        dir += new Vector3(Random.Range(-shootingSpreadVariance.x, shootingSpreadVariance.x),
            Random.Range(-shootingSpreadVariance.y, shootingSpreadVariance.y),
            Random.Range(-shootingSpreadVariance.z, shootingSpreadVariance.z)
            );
        return dir;
    }
    private IEnumerator SpawnTrailToMaxDistance(TrailRenderer trail, Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            time += Time.deltaTime / trailTime;
            yield return null;
        }
        trail.transform.position = targetPosition;
        //射到目标后延时一段时间回收
        yield return new WaitForSeconds(trail.time);
        MyObjectPool.Instance.ReturnObject(trail.gameObject);

        //Destroy(trail.gameObject, trail.time);
    }
    private IEnumerator SpawnTrail(TrailRenderer trail,RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
           trail.transform.position = Vector3.Lerp(startPosition,hit.point,time);
           time += Time.deltaTime/trailTime;
           yield return null;
        }
        trail.transform.position = hit.point; 
        Instantiate(metalImpactSystem, hit.point, Quaternion.LookRotation(hit.normal));
        //射到目标后延时一段时间回收
        yield return new WaitForSeconds(trail.GetComponent<TrailRenderer>().time);
        MyObjectPool.Instance.ReturnObject(trail.gameObject);
        //Destroy(trail.gameObject, trail.time); 
    }
 
}
