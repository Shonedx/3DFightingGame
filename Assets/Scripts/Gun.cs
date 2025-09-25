using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Gun : MonoBehaviour
{
    [SerializeField] private ParticleSystem metalImpactSystem;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TrailRenderer bulletsTrail; //子弹轨迹
    [SerializeField] private ParticleSystem shootingSystem;
    [SerializeField] private bool shootingSpread;
    [SerializeField] private Vector3 shootingSpreadVariance=new Vector3(.1f,.1f,.1f);
    [SerializeField] private LayerMask Mask;
    [SerializeField] private float lastShootTime;
    [SerializeField] private float shootCooldown;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [Header("射击距离")]
    [SerializeField] private float shootingMaxDistance;
    private float trailTime;
    private float hitHoleLifeTime=2f;
    public void Shoot()
    {
        if (lastShootTime + shootCooldown < Time.time)
        {
            shootingSystem.Play();
            Vector3 targetPosition;
            Vector3 direction = GetShootDirection(out targetPosition);
            if (Physics.Raycast(spawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
            {
                TrailRenderer trail = Instantiate(bulletsTrail, spawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
            }
            else
            {
                TrailRenderer trail = Instantiate(bulletsTrail, spawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrailToMaxDistance(trail,targetPosition));

            }
            lastShootTime = Time.time; //更新时间
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
        Debug.Log("targetPosition:" + targetPosition);
        Vector3 dir =targetPosition- spawnPoint.position;
        //dir += new Vector3(Random.Range(-shootingSpreadVariance.x, shootingSpreadVariance.x),
        //    Random.Range(-shootingSpreadVariance.y, shootingSpreadVariance.y),
        //    Random.Range(-shootingSpreadVariance.z, shootingSpreadVariance.z)
        //    );
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
        Destroy(trail.gameObject, trail.time);
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
        Destroy(trail.gameObject, trail.time); 
    }
    private IEnumerator DestroyHitHole(ParticleSystem hitHoleSystem)
    {
        yield return new WaitForSeconds(hitHoleLifeTime);
        Destroy(hitHoleSystem.gameObject);
    }
}
