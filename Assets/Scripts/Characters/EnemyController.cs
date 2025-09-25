using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody rb;
    [Header("‘À∂Ø")]
    public float motion_speed=1f;
    public float dirChangeGap=1.2f;
    public Vector3 moveDir;
    private Vector2 randomDir;

    
    private Vector3 record_velocity;
    private float record_angular_speed;
    private float time_scale;
    void Start()
    {
        GenerateMoveDir();
        rb = this.GetComponent<Rigidbody>();
        //TimeManager.Instance.RegisterObj(this.gameObject);
    }
    private void Update()
    {
        //time_scale = TimeManager.Instance.GetObjTimeScale(this.gameObject);
        //if (time_scale <= 0.01f)
        //{
        //    if (rb.isKinematic) return;
        //    record_velocity = rb.velocity;
        //    record_angular_speed = rb.angularVelocity.magnitude;
        //    rb.isKinematic = true;
        //}
        //else
        //{
        //    if (rb.isKinematic)
        //    {
        //        rb.isKinematic = false;
        //        rb.velocity = rb.velocity;
        //        rb.angularVelocity = rb.velocity.normalized * record_angular_speed;
        //    }
        //    rb.velocity *= time_scale;
        //    rb.angularVelocity *= time_scale;
        //}
        Invoke(nameof(GenerateMoveDir), dirChangeGap);
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void GenerateMoveDir()
    {
        randomDir = Random.insideUnitCircle;
        moveDir =new Vector3(randomDir.x, 0f, randomDir.y);
    }
    private void Move()
    {
        rb.AddForce(moveDir * motion_speed*10f*time_scale, ForceMode.Force);
    }
}
