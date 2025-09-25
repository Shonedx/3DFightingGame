using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class ShootingManager : MonoBehaviour
{
    [Header("参数")]
    public GameObject bullet;
    public float shotForce;
    public float upForce;
    public float reloadTime; //上弹时间
    public float perShotGapTime; //每次射击间隔
    public float repeatGapTime; //连发间隔
    public float spread; //子弹扩散
    public float maxShotDistance; // 最大射击距离
    public float bulletsDestroyTime;
    [Header("按键绑定")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode allowAutoShotKey = KeyCode.F;

    [Header("状态量")]
    public bool isAuto = true; //是否自动连发
    public bool allowButtonHold = true; //是否允许按住射击
    public bool isShooting=false;
    public bool isReloading=false;
    public bool readToShoot=true;
    public bool allowInvoke=true;
    [Header("射线检测")]
    public Transform rayOrigin;
    public Camera fpsCam;

    [Header("子弹信息")]
    public int bulletsMagazineCap; //弹夹容量
    public int bulletsPerTap; //连发个数
    public int leftBullets; //剩余子弹数
    public int shotBullets; //射出数量

    private ShootingEffectManager shotEffMngr;
    void Awake()
    {
        leftBullets = bulletsMagazineCap;
        readToShoot = true;
        shotEffMngr=GetComponentInChildren<ShootingEffectManager>();
    }

    void Update()
    {
    }
    //void MyInput()
    //{
    //    //shooting标志量来检测是否在射击状态
    //    if (allowButtonHold) isShooting = attackAction.IsPressed();
    //    else isShooting = attackAction.WasPressedThisFrame();
    //    if(reloadAction.WasPressedThisFrame() && leftBullets < bulletsMagazineCap && !isReloading) Reload();
    //    //if(allowButtonHold) isShooting=Input.GetKey(shootKey); 
    //    //else isShooting=Input.GetKeyDown(shootKey);

    //    //if (Input.GetKeyDown(reloadKey) && leftBullets < bulletsMagazineCap && !isReloading) Reload();
    //    if (isShooting && readToShoot && !isReloading && leftBullets <= 0) Reload(); //子弹射空自动装弹

    //    if (isShooting && readToShoot && !isReloading && leftBullets > 0)
    //    {
    //        shotBullets = 0;
    //        readToShoot = false;
    //        Shoot();
    //    }
    //}
    public void Shoot()
    {
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(.5f,.5f,0));
        RaycastHit rayHit;
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out rayHit))
        {
            targetPoint = rayHit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxShotDistance);
        }
        Vector3 dirWithoutSpread = targetPoint - rayOrigin.position;

        float xSpread = Random.Range(-spread, spread);
        float ySpread = Random.Range(-spread, spread);

        Vector3 dirWithSpread = dirWithoutSpread+new Vector3(xSpread,ySpread,0f);

        //实例化
        //GameObject currentBullet = Instantiate(bullet, rayOrigin.position, Quaternion.identity);
        //currentBullet.transform.forward = dirWithSpread.normalized; //把子弹转向为射击方向

        //currentBullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shotForce, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.up * upForce, ForceMode.Impulse);
        //Destroy(currentBullet, bulletsDestroyTime);
        
        //对象池
        GameObject currentBullet = BulletPoolManager.Instance.GetBullet();
        currentBullet.transform.position=rayOrigin.position;
        currentBullet.transform.rotation = Quaternion.identity;
        currentBullet.transform.forward=dirWithSpread.normalized; //把子弹转向为射击方向
        currentBullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shotForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.up * upForce, ForceMode.Impulse);
        //子弹的回收机制在子弹脚本中实现
        shotEffMngr.PlayEffects(); //播放射击特效
        leftBullets--;
        shotBullets++;
        if(allowInvoke&&(shotBullets>=bulletsPerTap||leftBullets<=0)) //子弹射完或者到达连发上限开始射击冷却
        {
            Invoke("ResetShoot", perShotGapTime);
            allowInvoke = false;
        }
        if (shotBullets < bulletsPerTap && leftBullets > 0)
        {
            Invoke("Shoot", repeatGapTime); 
        }
    }
    public void ResetShoot()
    {
        readToShoot = true;
        allowInvoke = true;
    }
    public void Reload()
    { 
        isReloading=true;
        Invoke("ReloadFinished", reloadTime);
    }
    public void ReloadFinished()
    {
        leftBullets = bulletsMagazineCap;
        isReloading = false;
    }
}
