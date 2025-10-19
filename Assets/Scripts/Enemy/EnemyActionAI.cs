using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemy
{
    public class EnemyActionAI : MonoBehaviour
    {
        private Transform enemyTransform;
        private Transform playerTransform;

        [Header("视野锁定参数")]
        [SerializeField] private float sightRange = 30f; //扇形视野范围 该变量为enemyforward的左右两侧偏移的角度 即（-sightRange,+sightRange）
        [SerializeField] private float sightDistance = 20f; //视野距离
        [SerializeField] private LayerMask obstacleMask; //障碍物层
        [SerializeField] private LayerMask playerMask; //玩家层
        
        [Header("攻击参数")]
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attackCooldown = 2f;
        [Header("巡逻参数")]
        private float dirChangeCounter=0f;
        private float dirChangeCoolDown=4f; //每次改变方向的时间间隔
        [SerializeField] private float rotateSpeed=2f;
        [SerializeField] private float walkSpeed=3f;
        [SerializeField] private float countSpeed=1f;

        Vector3 dir;
        Vector3 forwardDir;
        private void Start()
        {
            forwardDir= GetForwardDir();
            dir = forwardDir;
            enemyTransform = this.transform;   
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        private void Update()
        {
            forwardDir = GetForwardDir();
            RandomPatrol();
            Attack();
        }
        private Vector3 GetForwardDir()
        {
            return new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        }
        public void Attack()
        {
            if (IfTargetInSight())
            {
                Debug.Log("Attack Player");
            }
        }
        public void RandomPatrol()
        {
            if (dirChangeCounter >= dirChangeCoolDown)
            {
                dirChangeCounter = 0;
                dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            }
            dirChangeCounter += Time.deltaTime* countSpeed;
            Quaternion lookRotation= Quaternion.LookRotation(dir);
            enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation, lookRotation, Time.deltaTime * rotateSpeed); //平滑转向
            
            float angle = Vector3.Angle(dir, forwardDir);
            if(angle<=1f)
            enemyTransform.position += forwardDir * walkSpeed * Time.deltaTime;
        }

        public bool IfTargetInSight()
        {
            float distanceToTarget = Vector3.Distance(enemyTransform.position, playerTransform.position);
            if(distanceToTarget >= sightDistance)
                return false;
            Vector3 dirToTarget = (playerTransform.position - enemyTransform.position).normalized;
            float angleToTarget = Vector3.Angle(enemyTransform.forward, dirToTarget);
            if (angleToTarget > sightRange)
               return false;
            //dir = dirToTarget; //朝向玩家
            if (Physics.Linecast(enemyTransform.position + Vector3.up * .5f, playerTransform.position + Vector3.up * .5f, out RaycastHit hit, obstacleMask))
                return false;
            return true;
        }
    }
}

