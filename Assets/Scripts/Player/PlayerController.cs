using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(0)]

public class PlayerController : MonoBehaviour
{
    [Header("游戏输入")]
    [SerializeField] private GameInput gameInput;
    [Header("枪械")]
    [SerializeField] private Gun gun;
    [Header("手")] private Transform hand;
    private Transform orientation; //玩家朝向
    private Rigidbody rb; //玩家刚体
    #region PlayerParameters
    [SerializeField] private float playerHeight=1f;
    [SerializeField] private float playerGravity=10f;
    //ground&slope check
    private RaycastHit hit;
    [SerializeField]
    private bool isOnGround = true;
    private bool isOnSlope = false;
    private float maxSlopeAngle = 45f;
    //normal movement
    public bool isMoving; //是否在移动
    private float walkSpeed=7f;
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private Vector3 moveDir;
    //crouch
    private float crouchSpeed= 3f;
    private float crouchHeight = .5f;
    //sprint
    private float slideHeight = .6f;
    private float sprintSpeed = 10f;
    private bool isSprint = false;
    //slide 
    private float slideForce = 500f;
    private bool isSlide = false; //检测相关按键
    private bool sliding = false; //检测是否正在滑铲
    private float slideTime = .5f;
    private float slideTimer; 
    //jump 
    private float airSpeed=4f;
    private bool isJump = false;
    private bool readToJump = true;
    private float jumpForce = 5f;
    private float jumpCooldown = .5f;
    #endregion
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;
        orientation= this.GetComponent<Transform>();
        rb.freezeRotation = true;
        playerHeight = this.transform.localScale.y;
        crouchHeight= playerHeight *.6f;
        currentSpeed = walkSpeed;
        //TimeManager.Instance.RegisterObj(this.gameObject);
    }
    #region Update
    private void FixedUpdate()
    {
        AddGravity(); //施加重力
        GroundCheck(); //检测地面
    }
    private void Update()
    {
        if (gameInput.GetToJump()&&readToJump&&!isJump)//检测跳跃
        {
            Jump();
        }
        if(gameInput.GetCrouchPerformed()) //检测下蹲
        {
            Crouch();
        }
        else if(gameInput.GetCrouchCanceled())
        {
            StopCrouch();
        }
        if (gun.isAuto) //自动模式
        {
            if (gameInput.GetShotPressed()) //识别按键按住
                gun.ShootHandler();
        }
        else
        {
            if(gameInput.GetShotPerformed()) //识别按键按下
                gun.ShootHandler();
        }
        if (gameInput.GetToReload())
        {
            gun.Reload();
        }
        //检测滑铲
        isSlide = gameInput.GetToSlide();
        //检测疾跑
        isSprint = gameInput.GetToSprint(); 
        currentSpeed = isSprint?sprintSpeed:walkSpeed;
        Move(currentSpeed);
        if (isSlide&&!sliding)
        {
            Slide();
        }
    }
    #endregion
    #region Move

    private Vector3 SetMoveDir()
    {
        Vector2 inputVector2 = gameInput.GetMovementNormalized();
        Vector3 inputMoveDir = orientation.forward * inputVector2.y + orientation.right * inputVector2.x;
        return Vector3.ProjectOnPlane(inputMoveDir.normalized, hit.normal);
    }
    private void Move(float currentSpeed)
    {
        moveDir=SetMoveDir(); //设置moveDir

        float playerRadius = .7f;
        float moveDistance = currentSpeed * Time.deltaTime;
        Vector3 capsuleOffset = Vector3.up * transform.localScale.y * 0.5f;
        bool canMove = !Physics.CapsuleCast(transform.position - capsuleOffset, transform.position + capsuleOffset, playerRadius, moveDir, moveDistance);
        isMoving = moveDir != Vector3.zero;
        if (canMove)
        {
            transform.position += moveDir.normalized * moveDistance;
        }
        else
        {
            Vector3 moveDirXNormalized = new Vector3(moveDir.x, 0, 0).normalized;
            //检测x轴是否能动
            canMove = !Physics.CapsuleCast(transform.position - capsuleOffset, transform.position + capsuleOffset, playerRadius, moveDirXNormalized, moveDistance);
            if (canMove)
            {
                //如果只能动x轴
                transform.position += moveDirXNormalized * moveDistance;
            }
            else
            {
                //如果x轴不能动，检测z轴
                Vector3 moveDirZNormalized = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position - capsuleOffset, transform.position + capsuleOffset, playerRadius, moveDirZNormalized, moveDistance);
                if (canMove)
                {
                    //如果只能动z轴
                    transform.position += moveDirZNormalized * moveDistance;
                }
                else
                {
                    //什么都不处理
                }
            }
        }

    }
    public bool GetIfMoving() //检测是否正在移动
    {
        return isMoving;
    }
    #endregion
    #region Slide
    private void Slide()
    {
        SetHeight(slideHeight);
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        sliding = true;
        StartCoroutine(SlideDone());
    }
    private IEnumerator SlideDone()
    {
        rb.AddForce(moveDir.normalized * slideForce, ForceMode.Force);
        yield return new WaitUntil(() => //持续时长
        {

            slideTimer += Time.deltaTime;
            return isJump || slideTimer > slideTime; 
        });
        SetHeight(playerHeight); //恢复原高度
        sliding = false;
        slideTimer = 0;
    }
    #endregion
    #region Sprint
    private void Sprint()
    {
        currentSpeed = sprintSpeed;
    }
    #endregion
    #region Crouch
    private void Crouch()
    {
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        SetHeight(crouchHeight);
        currentSpeed = crouchSpeed;
    }
    private void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, playerHeight, transform.localScale.z);
        if (isOnGround)
        {
            currentSpeed = walkSpeed;
        }
    }
    #endregion
    #region Jump
    private void Jump()
    {
        currentSpeed = airSpeed;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //重置y轴速度
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        readToJump = false;
        isJump = true;
        StartCoroutine(JumpCooldown());
    }
    private IEnumerator JumpCooldown() //跳跃冷却
    {
        yield return new WaitForSeconds(jumpCooldown);
        isJump = false;
    }
    #endregion
    private void SetHeight(float targetHeight)
    {
        transform.localScale = new Vector3(transform.localScale.x,targetHeight,transform.localScale.z);
    }
    private void GroundCheck()
    {
        isOnGround = Physics.Raycast(transform.position, Vector3.down, out hit, .8f + playerHeight * 0.5f);
        if (isOnGround)
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle < maxSlopeAngle && angle != 0)
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
            }
        }
        else
        {
            isOnSlope = false;
        }
        readToJump = isOnGround;
    }
    void AddGravity()
    {
        rb.AddForce(Vector3.down * playerGravity, ForceMode.Acceleration);
    }


    //    private void TimeStopHandler()
    //    {
    //        //时间暂停
    //        if (Input.GetKeyDown(timeStopKey))
    //        {
    //            TimeManager.Instance.StartSlowDown();
    //        }
    //        if (Input.GetKeyDown(timeFlowKey))
    //        {
    //            TimeManager.Instance.StopSlowDown();
    //        }
    //    }

}
