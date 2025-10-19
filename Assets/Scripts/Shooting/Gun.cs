using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Health_Namespace;
public class Gun : MonoBehaviour
{
    // ����¼�ί��
    public delegate void ShootEventHandler();
    public event ShootEventHandler OnShoot;

    [SerializeField] private ParticleSystem metalImpactSystem; // ����ײ������Ч��
    [SerializeField] private Transform spawnPoint; // �ӵ������
    [SerializeField] private TrailRenderer bulletsTrail; // �ӵ��켣
    [SerializeField] private ParticleSystem shootingSystem; // �������Ч��

    [SerializeField] private CinemachineBrain cinemachineBrain; // cinemachine���������

    [Header("�������")]
    [SerializeField] private float shootingMaxDistance; // ����������
    [SerializeField] private bool shootingSpread; // �Ƿ����������ɢ
    [SerializeField] private Vector3 shootingSpreadVariance = new Vector3(.1f, .1f, .1f); // �����ɢ��Χ
    [SerializeField] private LayerMask Mask; // ���߼��㣨Ĭ��Ϊ���в㣩
    [SerializeField] private float lastShootTime; // �ϴ����ʱ�䣨�����Զ�����
    [SerializeField] private float shootCooldown;  // �Զ�������ȴʱ�䣨ÿ����������
    [SerializeField] private float reloadTime = 1f; // ����ʱ��
    [SerializeField] private int magzineCapacity = 30; // ��ϻ����
    [SerializeField] private int currentMagzineCapacity; // ��ǰ��ϻʣ������
    [SerializeField] private bool isMagzineNull = false; // ��ϻ�Ƿ�Ϊ��

    // �������
    [SerializeField] public bool isAuto; // �Ƿ�Ϊ�Զ����ģʽ
    [SerializeField] private int burstCount; // �������
    [SerializeField] private int currentBurstCount = 0; // ��ǰ�������
    [SerializeField] private float shootingGaptime = .2f; // ������ʱ��
    private bool readyToShoot = true; // �Ƿ�׼�������

    [Header("�ӵ��켣����ʱ��")]
    [SerializeField] private float trailTime; // �켣��ʾʱ��

    private void Awake()
    {
        currentMagzineCapacity = magzineCapacity;
        OnShoot += DecreaseCurrentMagzineCapacity;
    }

    private void OnDestroy()
    {
        // �Ƴ��¼���������ֹ�ڴ�й©
        OnShoot -= DecreaseCurrentMagzineCapacity;
    }

    private void Update()
    {
        isMagzineNull = currentMagzineCapacity < 0;
    }

    /// <summary>
    /// ���������
    /// </summary>
    public void ShootHandler()
    {
        // ���µ�ϻ״̬
        if (currentMagzineCapacity > 0)
            isMagzineNull = false;
        else
        {
            currentMagzineCapacity = 0;
            isMagzineNull = true;
        }

        // �����ϻ��Ϊ��
        if (!isMagzineNull)
        {
            // �Զ����ģʽ����ȴ���
            if (isAuto && (lastShootTime + shootCooldown < Time.time))
            {
                Shoot();
                lastShootTime = Time.time; // �������ʱ��
            }
            // ���Զ�ģʽ��׼������������ģʽ��
            else if (!isAuto && readyToShoot)
            {
                currentBurstCount++;
                Shoot();
                readyToShoot = false; // ��ֹ��һ�����
                StartCoroutine(BurstShot());
            }
        }
        // ��ϻΪ��ʱ����
        else
        {
            Invoke(nameof(Reload), reloadTime);
        }
    }

    /// <summary>
    /// ��ȡ��ϻ������
    /// </summary>
    public int GetMagzineCap()
    {
        return magzineCapacity;
    }

    /// <summary>
    /// ��ȡ��ǰ��ϻʣ������
    /// </summary>
    public int GetCurrentMagzineCap()
    {
        return currentMagzineCapacity;
    }

    /// <summary>
    /// ����װ��
    /// </summary>
    public void Reload()
    {
        currentMagzineCapacity = magzineCapacity;
        isMagzineNull = false; // ��ϻ��Ϊ��
    }

    /// <summary>
    /// ���ٵ�ǰ��ϻ����
    /// </summary>
    private void DecreaseCurrentMagzineCapacity()
    {
        if (currentMagzineCapacity > 0)
            currentMagzineCapacity--;
    }

    /// <summary>
    /// ����Э��
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
    /// ִ���������
    /// </summary>
    public void Shoot()
    {
        // ��������¼�
        OnShoot?.Invoke();

        // ������������Ŀ��λ��
        Vector3 targetPosition;
        Vector3 direction = GetShootDirection(out targetPosition);

        // ���߼��
        if (Physics.Raycast(spawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
        {
            GameObject target= hit.collider.gameObject;
            if(target!= null&&target.CompareTag("Enemy"))
            PlayerDamage(10, target); // �����е�Ŀ������˺�
            // �Ӷ���ػ�ȡ�켣����
            GameObject trailObj = MyObjectPool.Instance.GetObject();
            TrailRenderer trail = trailObj.GetComponent<TrailRenderer>();
            trail.transform.position = spawnPoint.position;
            StartCoroutine(SpawnTrail(trail, hit));
        }
        else
        {
            // �Ӷ���ػ�ȡ�켣����
            GameObject trailObj = MyObjectPool.Instance.GetObject();
            TrailRenderer trail = trailObj.GetComponent<TrailRenderer>();
            trail.transform.position = spawnPoint.position;
            StartCoroutine(SpawnTrailToMaxDistance(trail, targetPosition));
        }
    }
    ///<summary>
    ///ִ��Damage
    /// </summary>
    void PlayerDamage(int hurtValue, GameObject gameObject)
    {
        gameObject.GetComponent<Health>().TakeDamage(hurtValue);
    }
    /// <summary>
    /// ��ȡ�������
    /// </summary>
    /// <param name="targetPosition">���Ŀ��λ��</param>
    /// <returns>�����������</returns>
    private Vector3 GetShootDirection(out Vector3 targetPosition)
    {
        Camera currentCamera = cinemachineBrain.OutputCamera;
        // ��������ķ�������
        Ray ray = currentCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;

        // Ĭ��Ŀ��λ��Ϊ�����̴�
        targetPosition = ray.GetPoint(shootingMaxDistance);

        // ������߻������壬����Ŀ��λ��
        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
        }

        // ����ӷ���㵽Ŀ��λ�õķ���
        Vector3 dir = targetPosition - spawnPoint.position;

        // Ӧ�������ɢ
        dir += new Vector3(
            Random.Range(-shootingSpreadVariance.x, shootingSpreadVariance.x),
            Random.Range(-shootingSpreadVariance.y, shootingSpreadVariance.y),
            Random.Range(-shootingSpreadVariance.z, shootingSpreadVariance.z)
        );

        return dir;
    }
    /// <summary>
    /// ���ɵ���������ӵ��켣
    /// </summary>
    private IEnumerator SpawnTrailToMaxDistance(TrailRenderer trail, Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        // ��ֵ�ƶ��켣
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            time += Time.deltaTime / trailTime;
            yield return null;
        }

        trail.transform.position = targetPosition;

        // �ȴ��켣��ʧ����ն���
        yield return new WaitForSeconds(trail.time);
        MyObjectPool.Instance.ReturnObject(trail.gameObject);
    }

    /// <summary>
    /// ���ɻ���Ŀ����ӵ��켣
    /// </summary>
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        // ��ֵ�ƶ��켣
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trailTime;
            yield return null;
        }

        trail.transform.position = hit.point;
        // ����ײ������Ч��
        Instantiate(metalImpactSystem, hit.point, Quaternion.LookRotation(hit.normal));

        // �ȴ��켣��ʧ����ն���
        yield return new WaitForSeconds(trail.GetComponent<TrailRenderer>().time);
        MyObjectPool.Instance.ReturnObject(trail.gameObject);
    }
}
