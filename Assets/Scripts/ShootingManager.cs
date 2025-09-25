using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class ShootingManager : MonoBehaviour
{
    [Header("����")]
    public GameObject bullet;
    public float shotForce;
    public float upForce;
    public float reloadTime; //�ϵ�ʱ��
    public float perShotGapTime; //ÿ��������
    public float repeatGapTime; //�������
    public float spread; //�ӵ���ɢ
    public float maxShotDistance; // ����������
    public float bulletsDestroyTime;
    [Header("������")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode allowAutoShotKey = KeyCode.F;

    [Header("״̬��")]
    public bool isAuto = true; //�Ƿ��Զ�����
    public bool allowButtonHold = true; //�Ƿ�����ס���
    public bool isShooting=false;
    public bool isReloading=false;
    public bool readToShoot=true;
    public bool allowInvoke=true;
    [Header("���߼��")]
    public Transform rayOrigin;
    public Camera fpsCam;

    [Header("�ӵ���Ϣ")]
    public int bulletsMagazineCap; //��������
    public int bulletsPerTap; //��������
    public int leftBullets; //ʣ���ӵ���
    public int shotBullets; //�������

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
    //    //shooting��־��������Ƿ������״̬
    //    if (allowButtonHold) isShooting = attackAction.IsPressed();
    //    else isShooting = attackAction.WasPressedThisFrame();
    //    if(reloadAction.WasPressedThisFrame() && leftBullets < bulletsMagazineCap && !isReloading) Reload();
    //    //if(allowButtonHold) isShooting=Input.GetKey(shootKey); 
    //    //else isShooting=Input.GetKeyDown(shootKey);

    //    //if (Input.GetKeyDown(reloadKey) && leftBullets < bulletsMagazineCap && !isReloading) Reload();
    //    if (isShooting && readToShoot && !isReloading && leftBullets <= 0) Reload(); //�ӵ�����Զ�װ��

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

        //ʵ����
        //GameObject currentBullet = Instantiate(bullet, rayOrigin.position, Quaternion.identity);
        //currentBullet.transform.forward = dirWithSpread.normalized; //���ӵ�ת��Ϊ�������

        //currentBullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shotForce, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.up * upForce, ForceMode.Impulse);
        //Destroy(currentBullet, bulletsDestroyTime);
        
        //�����
        GameObject currentBullet = BulletPoolManager.Instance.GetBullet();
        currentBullet.transform.position=rayOrigin.position;
        currentBullet.transform.rotation = Quaternion.identity;
        currentBullet.transform.forward=dirWithSpread.normalized; //���ӵ�ת��Ϊ�������
        currentBullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shotForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.up * upForce, ForceMode.Impulse);
        //�ӵ��Ļ��ջ������ӵ��ű���ʵ��
        shotEffMngr.PlayEffects(); //���������Ч
        leftBullets--;
        shotBullets++;
        if(allowInvoke&&(shotBullets>=bulletsPerTap||leftBullets<=0)) //�ӵ�������ߵ����������޿�ʼ�����ȴ
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
