using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Health_Namespace;
public class Gun : MonoBehaviour
{
    // 射击事件委托
    public delegate void ShootEventHandler();
    public event ShootEventHandler OnShoot;

    [SerializeField] private ParticleSystem metalImpactSystem; // 金属撞击粒子效果
    [SerializeField] private Transform spawnPoint; // 子弹发射点
    [SerializeField] private TrailRenderer bulletsTrail; // 子弹轨迹
    [SerializeField] private ParticleSystem shootingSystem; // 射击粒子效果

    [SerializeField] private CinemachineBrain cinemachineBrain; // cinemachine相机控制器

    [Header("射击参数")]
    [SerializeField] private float shootingMaxDistance; // 最大射击距离
    [SerializeField] private bool shootingSpread; // 是否启用射击扩散
    [SerializeField] private Vector3 shootingSpreadVariance = new Vector3(.1f, .1f, .1f); // 射击扩散范围
    [SerializeField] private LayerMask Mask; // 射线检测层（默认为所有层）
    [SerializeField] private float lastShootTime; // 上次射击时间（用于自动开火）
    [SerializeField] private float shootCooldown;  // 自动开火冷却时间（每次射击间隔）
    [SerializeField] private float reloadTime = 1f; // 换弹时间
    [SerializeField] private int magzineCapacity = 30; // 弹匣容量
    [SerializeField] private int currentMagzineCapacity; // 当前弹匣剩余数量
    [SerializeField] private bool isMagzineNull = false; // 弹匣是否为空

    // 连射参数
    [SerializeField] public bool isAuto; // 是否为自动射击模式
    [SerializeField] private int burstCount; // 连射次数
    [SerializeField] private int currentBurstCount = 0; // 当前连射计数
    [SerializeField] private float shootingGaptime = .2f; // 连射间隔时间
    private bool readyToShoot = true; // 是否准备好射击

    [Header("子弹轨迹持续时间")]
    [SerializeField] private float trailTime; // 轨迹显示时间

    private void Awake()
    {
        currentMagzineCapacity = magzineCapacity;
        OnShoot += DecreaseCurrentMagzineCapacity;
    }

    private void OnDestroy()
    {
        // 移除事件监听，防止内存泄漏
        OnShoot -= DecreaseCurrentMagzineCapacity;
    }

    private void Update()
    {
        isMagzineNull = currentMagzineCapacity < 0;
    }

    /// <summary>
    /// 射击处理函数
    /// </summary>
    public void ShootHandler()
    {
        // 更新弹匣状态
        if (currentMagzineCapacity > 0)
            isMagzineNull = false;
        else
        {
            currentMagzineCapacity = 0;
            isMagzineNull = true;
        }

        // 如果弹匣不为空
        if (!isMagzineNull)
        {
            // 自动射击模式且冷却完成
            if (isAuto && (lastShootTime + shootCooldown < Time.time))
            {
                Shoot();
                lastShootTime = Time.time; // 更新射击时间
            }
            // 非自动模式且准备就绪（连射模式）
            else if (!isAuto && readyToShoot)
            {
                currentBurstCount++;
                Shoot();
                readyToShoot = false; // 阻止下一次射击
                StartCoroutine(BurstShot());
            }
        }
        // 弹匣为空时换弹
        else
        {
            Invoke(nameof(Reload), reloadTime);
        }
    }

    /// <summary>
    /// 获取弹匣总容量
    /// </summary>
    public int GetMagzineCap()
    {
        return magzineCapacity;
    }

    /// <summary>
    /// 获取当前弹匣剩余数量
    /// </summary>
    public int GetCurrentMagzineCap()
    {
        return currentMagzineCapacity;
    }

    /// <summary>
    /// 重新装弹
    /// </summary>
    public void Reload()
    {
        currentMagzineCapacity = magzineCapacity;
        isMagzineNull = false; // 弹匣不为空
    }

    /// <summary>
    /// 减少当前弹匣数量
    /// </summary>
    private void DecreaseCurrentMagzineCapacity()
    {
        if (currentMagzineCapacity > 0)
            currentMagzineCapacity--;
    }

    /// <summary>
    /// 连射协程
    /// </summary>
    private IEnumerator BurstShot()
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
    }

    /// <summary>
    /// 执行射击操作
    /// </summary>
    public void Shoot()
    {
        // 触发射击事件
        OnShoot?.Invoke();

        // 计算射击方向和目标位置
        Vector3 targetPosition;
        Vector3 direction = GetShootDirection(out targetPosition);

        // 射线检测
        if (Physics.Raycast(spawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
        {
            GameObject target= hit.collider.gameObject;
            if(target!= null&&target.CompareTag("Enemy"))
            PlayerDamage(10, target); // 对命中的目标造成伤害
            // 从对象池获取轨迹对象
            GameObject trailObj = MyObjectPool.Instance.GetObject();
            TrailRenderer trail = trailObj.GetComponent<TrailRenderer>();
            trail.transform.position = spawnPoint.position;
            StartCoroutine(SpawnTrail(trail, hit));
        }
        else
        {
            // 从对象池获取轨迹对象
            GameObject trailObj = MyObjectPool.Instance.GetObject();
            TrailRenderer trail = trailObj.GetComponent<TrailRenderer>();
            trail.transform.position = spawnPoint.position;
            StartCoroutine(SpawnTrailToMaxDistance(trail, targetPosition));
        }
    }
    ///<summary>
    ///执行Damage
    /// </summary>
    void PlayerDamage(int hurtValue, GameObject gameObject)
    {
        gameObject.GetComponent<Health>().TakeDamage(hurtValue);
    }
    /// <summary>
    /// 获取射击方向
    /// </summary>
    /// <param name="targetPosition">输出目标位置</param>
    /// <returns>射击方向向量</returns>
    private Vector3 GetShootDirection(out Vector3 targetPosition)
    {
        Camera currentCamera = cinemachineBrain.OutputCamera;
        // 从相机中心发射射线
        Ray ray = currentCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;

        // 默认目标位置为最大射程处
        targetPosition = ray.GetPoint(shootingMaxDistance);

        // 如果射线击中物体，更新目标位置
        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
        }

        // 计算从发射点到目标位置的方向
        Vector3 dir = targetPosition - spawnPoint.position;

        // 应用射击扩散
        dir += new Vector3(
            Random.Range(-shootingSpreadVariance.x, shootingSpreadVariance.x),
            Random.Range(-shootingSpreadVariance.y, shootingSpreadVariance.y),
            Random.Range(-shootingSpreadVariance.z, shootingSpreadVariance.z)
        );

        return dir;
    }
    /// <summary>
    /// 生成到最大距离的子弹轨迹
    /// </summary>
    private IEnumerator SpawnTrailToMaxDistance(TrailRenderer trail, Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        // 插值移动轨迹
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            time += Time.deltaTime / trailTime;
            yield return null;
        }

        trail.transform.position = targetPosition;

        // 等待轨迹消失后回收对象
        yield return new WaitForSeconds(trail.time);
        MyObjectPool.Instance.ReturnObject(trail.gameObject);
    }

    /// <summary>
    /// 生成击中目标的子弹轨迹
    /// </summary>
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        // 插值移动轨迹
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trailTime;
            yield return null;
        }

        trail.transform.position = hit.point;
        // 生成撞击粒子效果
        Instantiate(metalImpactSystem, hit.point, Quaternion.LookRotation(hit.normal));

        // 等待轨迹消失后回收对象
        yield return new WaitForSeconds(trail.GetComponent<TrailRenderer>().time);
        MyObjectPool.Instance.ReturnObject(trail.gameObject);
    }
}
