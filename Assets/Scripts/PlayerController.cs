using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    //[Header("�������")]
    //public float playHeight;
    //[Header("�ƶ��ٶ�")]
    //public float walkSpeed = 4f;
    //public float sprintSpeed = 7f;
    //[Header("��ǰ�ٶ�")]
    //public float currentSpeed;
    //[Header("����")]
    //public float slideSpeed = 12f;
    //public float slideForce = 500f;
    //public float slideDuration = .25f;
    //private float slideTimer;
    //public float slideHeight;
    //public bool sliding=false;
    //[Header("�¶�")]
    //public float crouchSpeed = 3f;
    //public float crouchHeight;
    //[Header("��Ծ")]
    //public float jumpForce;
    //public float jumpCooldown; //��ȴʱ��
    //private bool readyToJump=true;
    //public float airSpeed = 3f;
    //private bool jumping = false;
    [Header("б��")]
    public LayerMask ifIsSlope;
    public float maxSlopeAngle;
    public bool sloping = false;
    private RaycastHit slopeHit; //б��б��
    public Vector3 slopeDir; //moveDirͶӰ��б�µ�����
    [Header("������")]
    public KeyCode jumpKey=KeyCode.Space;
    public KeyCode timeStopKey=KeyCode.K;
    public KeyCode timeFlowKey=KeyCode.J;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode slideKey = KeyCode.X;
    //[Header("���������")]
    //public LayerMask ifIsGround;
    //public float groudDrag;
    //private bool grouded = true;
    //
    private Rigidbody rb;
    //private Vector3 moveDir;
    //public Transform orientation;
    //private float inputHorizontal;
    //private float inputVertical;


    [Header("�˶����")]
    public MovementManager mM;
    public SlideMotion sm;
    public enum MotionState
    {
        walk,
        sprint,
        crouching, //�¶�
        slide,
        air,
    };
    [Header("�˶�״̬")]
    public MotionState playerState;

    void Start()
    {
        rb=this.GetComponent<Rigidbody>();
        mM = this.GetComponent<MovementManager>();
        sm = this.GetComponent<SlideMotion>();
        if(mM==null)
        {
            Debug.LogError("MovementManager���δ�ҵ�");
        }
        if (sm == null)
        {
            Debug.LogError("SlideMotion���δ�ҵ�");
        }
        rb.freezeRotation = true;
        TimeManager.Instance.RegisterObj(this.gameObject);
    }

    private void Update()
    {
        //myInput();
        //grouded = Physics.Raycast(this.transform.position, Vector3.down, playHeight * 0.5f + 0.8f, ifIsGround);
        //if (grouded)
        //{
        //    rb.drag = groudDrag;
        //}
        //else if(!grouded&&!sloping)
        //{
        //    playerState = MotionState.air;
        //    currentSpeed = airSpeed;
        //    rb.drag = 0;
        //}
        //else if(sloping && !grouded)
        //{
        //    rb.drag = 0;
        //}
        InputHandler();
        mM.GroundInspector();
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        mM.LimitedSpeed(mM.currentSpeed);
        mM.Move(mM.currentSpeed);
        //SpeedControl(currentSpeed);
        //Move(currentSpeed);
    }
    private void InputHandler()
    {
        mM.MoveInput();
        if (Input.GetKeyDown(crouchKey))
        {
            mM.StartCrouch();
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            mM.StopCrouch();
        }
        if (Input.GetKey(sprintKey))
        {
            mM.currentSpeed = mM.sprintSpeed;
        }
        else if (!Input.GetKey(sprintKey) && !sm.isSlide)
        {
            mM.currentSpeed = mM.walkSpeed;
        }
        if (Input.GetKeyDown(slideKey)) //slide
        {
            sm.Slide(mM.moveDir);
        }
        //��Ծ
        if (Input.GetKeyDown(jumpKey) && (mM.isInGround || sloping) && !mM.isJump)
        {
            mM.Jump();
        }
    }
    //    private void SpeedControl(float maxSpeed)
    //    {
    //        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    //        if (flatVel.magnitude > maxSpeed)
    //        {
    //            Vector3 limitedVel = flatVel.normalized * maxSpeed;
    //            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
    //        }
    //    }
    //    private void Move(float speed)
    //    { 
    //        moveDir = orientation.forward* inputVertical + orientation.right * inputHorizontal;
    //        if (sloping)
    //        {
    //            slopeDir = GetSlopeDir(moveDir);
    //            float addedForce = speed * 10f;
    //            rb.AddForce(slopeDir.normalized * addedForce, ForceMode.Force);
    //            if (readyToJump)
    //            {
    //                rb.AddForce(-slopeHit.normal * addedForce*0.8f, ForceMode.Force);
    //            }
    //        }
    //        else
    //        {
    //            rb.AddForce(moveDir.normalized * speed * 10f, ForceMode.Force);
    //        }
    //    }
    //    private void myInput()
    //    {
    //        inputHorizontal = Input.GetAxis("Horizontal");
    //        inputVertical = Input.GetAxis("Vertical");

    //        TimeStopHandler();
    //        StateHandler();
    //    }
    //    private bool Onslope()
    //    {
    //        if(Physics.Raycast(transform.position,Vector3.down,out slopeHit,playHeight*0.5f+0.8f, ifIsSlope))
    //        {
    //            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
    //            Debug.Log("angle:" + angle);
    //            return angle < maxSlopeAngle&&angle!=0;
    //        }
    //        return false;
    //    }
    //    private Vector3 GetSlopeDir(Vector3 moveDir)
    //    {
    //       return  Vector3.ProjectOnPlane(moveDir.normalized, slopeHit.normal);
    //    }

    //    private void StateHandler()
    //    {
    //        CrouchHandler();
    //        //��̺�����
    //        if (Input.GetKey(sprintKey))
    //        {
    //            currentSpeed = sprintSpeed;
    //            playerState = MotionState.sprint;
    //        }
    //        else if(!Input.GetKey(sprintKey)&&!sliding)
    //        {
    //            playerState = MotionState.walk;
    //            currentSpeed = walkSpeed;
    //        }
    //        if (Input.GetKeyDown(slideKey)) //slide
    //        {
    //            transform.localScale = new Vector3(transform.localScale.x, slideHeight, transform.localScale.z);
    //            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    //            sliding = true;
    //            currentSpeed = slideSpeed;
    //            slideTimer = slideDuration;
    //            playerState = MotionState.slide;
    //            StartCoroutine(SlideHandler(moveDir));
    //        }
    //        //��Ծ
    //        if (Input.GetKeyDown(jumpKey) && (grouded||sloping) && readyToJump)
    //        {
    //            Jump();
    //            Invoke(nameof(ResetJump), jumpCooldown); // ��Ծ��ȴ
    //        }
    //        sloping = Onslope();
    //    }
    //    private void CrouchHandler()
    //    {
    //        if (Input.GetKeyDown(crouchKey))
    //        {
    //            currentSpeed = crouchSpeed;
    //            playerState = MotionState.crouching;
    //            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
    //            rb.AddForce(Vector3.down * 10f * 5, ForceMode.Impulse);
    //        }
    //        else if (Input.GetKeyUp(crouchKey))
    //        {
    //            transform.localScale = new Vector3(transform.localScale.x, playHeight, transform.localScale.z);
    //        }
    //    }
    //    private void TimeStopHandler()
    //    {
    //        //ʱ����ͣ
    //        if (Input.GetKeyDown(timeStopKey))
    //        {
    //            TimeManager.Instance.StartSlowDown();
    //        }
    //        if (Input.GetKeyDown(timeFlowKey))
    //        {
    //            TimeManager.Instance.StopSlowDown();
    //        }
    //    }
    //    //slide
    //    private float SlideDelay(float timer)
    //    {
    //        return timer -= Time.deltaTime;
    //    }
    //    private IEnumerator SlideHandler(Vector3 slideDir)
    //    {
    //        rb.AddForce(slideDir.normalized * slideForce, ForceMode.Force);
    //        //yield return new WaitForSeconds(slideDuration);
    //        yield return new WaitUntil(() => //�ȴ�����
    //        {
    //            slideTimer = SlideDelay(slideTimer);
    //            return (slideTimer < 0 || jumping);
    //        });
    //        transform.localScale = new Vector3(transform.localScale.x, playHeight, transform.localScale.z);
    //        sliding = false;
    //    }
    //    //Jump

    //    private void Jump()
    //    { 
    //        rb.velocity=new Vector3(rb.velocity.x,0,rb.velocity.z);
    //        rb.AddForce(jumpForce*Vector3.up, ForceMode.Impulse);
    //        jumping = true;
    //        readyToJump = false;
    //    }
    //    private void ResetJump()
    //    {
    //        jumping = false;
    //        readyToJump = true;
    //    }
}
