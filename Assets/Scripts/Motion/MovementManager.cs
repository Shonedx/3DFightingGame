using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class MovementManager : MonoBehaviour
{
    [Header("�������")]
    public float playerHeight;
    public float crouchHeight;
    public float gravityAccel=10f;
    [Header("��ǰ")]
    public Vector3 moveDir;
    public float currentSpeed;
    [Header("Walk")]
    public bool isWalk = true;
    public float walkSpeed=4f;
    [Header("Sprint")]
    public bool isSprint=false;
    public float sprintSpeed=12f;
    [Header("Crouch")]
    public bool isCrouch=false;
    public float crouchSpeed = 3f;
    [Header("Jump")]
    public bool isJump = false;
    public bool readToJump = true;
    public float jumpForce=10f;
    public float jumpCooldown=.5f; //��ȴʱ��
    public float airSpeed = 3f;
    [Header("���������")]
    public LayerMask GroundMask;
    public float groudDrag=5f;
    public bool isOnGround = true;
    //
    public Rigidbody rb;
    public Transform orientation;
    public SlopeMotion slpM;
    public SlideMotion sm;
    public float inputHorizontal;
    public float inputVertical;
    private void Awake()
    {
       
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        slpM = GetComponent<SlopeMotion>();
        sm = this.GetComponent<SlideMotion>();
        orientation = this.transform;
        playerHeight = this.transform.localScale.y;
        DisableGravity();
    }
    private void Update()
    {
    }
    private void FixedUpdate()
    {
        GroundInspector();
        AddCustomGravity();
        LimitedSpeed(currentSpeed);
        Move(currentSpeed);
    }
    public void DisableGravity()
    {
        if (rb != null)
        {
            rb.useGravity = false;
        }
        else
        {
            Debug.LogError("δʶ��RigitBody!");
        }
    }
    public void  AddCustomGravity()
    {
        rb.AddForce(Vector3.down * gravityAccel, ForceMode.Acceleration);
    }
    public void Move(float speed)
    {
        if(orientation == null)
        {
            Debug.Log("orientation is null ");
            orientation = this.transform;
        }
        moveDir = (orientation.forward * inputVertical + orientation.right * inputHorizontal).normalized;
        if (sm == null||!slpM.isOnSlope)
        {
            this.transform.position+=new Vector3(moveDir.x,transform.position.y,moveDir.z) * speed * Time.fixedDeltaTime;
            //rb.AddForce(moveDir.normalized * speed * 10f, ForceMode.Force);
        }
        else if(slpM.isOnSlope) //ִ��б���߼�
        {
            float addedForce = speed * 10f;
            slpM.SetSlopeDir(moveDir);
            this.transform.position += slpM.slopeDir.normalized * speed * Time.fixedDeltaTime;
            //rb.AddForce(slpM.slopeDir.normalized * addedForce, ForceMode.Force);
            if (!isJump)
            {
                rb.AddForce(-slpM.slopeHit.normal * addedForce * 0.8f, ForceMode.Force);
            }
        }
    }
    public void LimitedSpeed(float maxSpeed)
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z); //ˮƽ�ٶ�
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    public void CheckGround()
    {
        isOnGround = Physics.Raycast(this.transform.position, Vector3.down, 0.5f*playerHeight  + 0.8f, GroundMask);
    }
    public void GroundInspector() //����Ƿ��ڵ���
    {
        CheckGround();
        if (sm == null)
        {
            if (isOnGround)
            {
                rb.drag = groudDrag;
            }
            else
            {
                currentSpeed = airSpeed;
                rb.drag = 0;
            }
        }
        else //����б�����ʱ
        {
            slpM.CheckSlope();
            if (isOnGround)
            {
                rb.drag = groudDrag;
            }
            else if (!isOnGround && !slpM.isOnSlope)
            {
                currentSpeed = airSpeed;
                rb.drag = 0;
            }
            else if (slpM.isOnSlope && !isOnGround)
            {
                rb.drag = groudDrag;
                //������������
            }
        }
        if((isOnGround|| slpM.isOnSlope)&&!isJump)
            readToJump= true;
        //Debug.Log("Ground:" + isOnGround + "Slope:" + sm.isOnSlope);
    }
    public void StartCrouch()
    {
        currentSpeed = crouchSpeed;
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        rb.AddForce(Vector3.down * 10f * 5, ForceMode.Impulse);
    }
    public void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, playerHeight, transform.localScale.z);
    }
    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        isJump = true;
        readToJump = false;
        StartCoroutine(JumpDelay(jumpCooldown));
    }
    private IEnumerator JumpDelay(float delayedTime)
    {
        yield return new WaitForSeconds(delayedTime);
        ResetJump();
    }
    public void ResetJump()
    {
        isJump = false;
    }

}
