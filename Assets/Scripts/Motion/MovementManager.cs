using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MovementManager : MonoBehaviour
{
    [Header("玩家属性")]
    public float playerHeight;
    public float crouchHeight;
    [Header("当前")]
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
    public float jumpForce=10f;
    public float jumpCooldown=.5f; //冷却时间
    public float airSpeed = 3f;
    [Header("地面检测相关")]
    public LayerMask GroundMask;
    public float groudDrag=5f;
    public bool isInGround = true;
    //
    private Rigidbody rb;
    private Transform orientation;
    private float inputHorizontal;
    private float inputVertical;
    void Start()
    {
        rb = GetComponent<Rigidbody>();    
        playerHeight= transform.localScale.y;
        crouchHeight=.5f*transform.localScale.y;
    }
    public void MoveInput()
    {
        inputHorizontal=Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
    }
    public void Move(float speed)
    {
        if(orientation == null)
        {
            Debug.Log("orientation is null ");
            orientation = this.transform;
        }
        moveDir = orientation.forward * inputVertical + orientation.right * inputHorizontal;
        rb.AddForce(moveDir.normalized * speed * 10f, ForceMode.Force);
    }
    public void LimitedSpeed(float maxSpeed)
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z); //水平速度
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    public void GroundInspector() //检测是否在地面
    {
        isInGround = Physics.Raycast(transform.position, Vector3.down, playerHeight  + .2f, GroundMask);
        if (isInGround)
        {
            rb.drag = groudDrag;
        }
        else if(!isInGround)
        {
            currentSpeed = airSpeed;
            rb.drag = 0;
        }
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
