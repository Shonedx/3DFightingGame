using Motion.Interface;
using System;
using System.Collections;
using UnityEngine;

namespace Motion.Movement
{

    public class Movement
    {
        //属性

        //地面，斜面检测
        public bool IsOnGround { get { return _isOnGround; } }
        public bool IsOnSlope { get { return _isOnSlope; } }
        public float MaxSlopeAngle { get { return _maxSlopeAngle; } set { _maxSlopeAngle = value; } }
        public float CrouchSpeed { get { return _crouchSpeed; } set { _crouchSpeed = value; } }
        public float CrouchHeight { get { return _crouchHeight; } set { _crouchHeight = value; } }
        public RaycastHit Hit { get { return _hit; } }
        //滑铲属性
        public bool IsSlide { get { return _isSlide; } }
        public bool Sliding { get { return _sliding; } }
        public float SlideForce { get { return _slideForce; } set { _slideForce = value; } }
        public float SlideTime { get { return _slideTime; } set { _slideTime = value; } }
        public float SlideHeight { get { return _slideHeight; } set { _slideHeight = value; } }
        //跳跃属性
        public bool IsJump { get { return _isJump; } }
        public bool ReadToJump { get { return _readToJump; } }
        public float AirSpeed { get { return _airSpeed; } set { _airSpeed = value; } }
        public float JumpForce { get { return _jumpForce; } set { _jumpForce = value; } }
        public float JumpCooldown { get { return _jumpCooldown; } set { _jumpCooldown = value; } }
        //对象属性
        public Rigidbody ObjRigidBody { get { return _objRigidBody; } set { _objRigidBody = value; } }
        public Transform ObjTransform { get { return _objTransform; } set { _objTransform = value; } }
        public float ObjHeight { get { return _objHeight; } set { _objHeight = value; } }
        public float ObjGravity { get { return _objGravity; } set { _objGravity = value; } }
        //移动属性
        public Vector3 MoveDir { get { return _moveDir; } }
        public Vector3 TargetDir { get { return _targetDir; } set { _targetDir = value; } }
        public float WalkSpeed { get { return _walkSpeed; } set { _walkSpeed = value; } }
        public bool IsSprint { get { return _isSprint; } }
        public float SprintSpeed { get { return _sprintSpeed; } set { _sprintSpeed = value; } }

        //对象参数
        private Transform _objTransform;
        private Rigidbody _objRigidBody; //玩家刚体
        private float _objHeight = 1f;
        private float _objGravity = 10f;
        //移动参数
        private Vector3 _moveDir; //移动方向
        private float currentSpeed;
        private Vector3 moveDir;
        private float _walkSpeed = 7f;
        private bool isMoving; //是否在移动
        private Vector3 _targetDir;
        //冲刺参数
        private float _sprintSpeed = 10f;
        private bool _isSprint = false;

        public Movement(Transform objTransform, Rigidbody objRigidBody, Vector3 targetDir)
        {
            _objTransform = objTransform;
            _objRigidBody = objRigidBody;
            _targetDir = targetDir;
        }
        public void Init()
        {
            if (_objTransform == null)
                throw new InvalidOperationException("ObjTransform is null in Movement.cs");
            if (_objRigidBody == null)
                throw new InvalidOperationException("ObjRigidBody is null in Movement.cs");
            _objRigidBody.useGravity = false;
            _objRigidBody.freezeRotation = true;
            _objHeight = _objTransform.localScale.y;
            _crouchHeight = _objHeight * .6f;
            currentSpeed = _walkSpeed;
        }
        //私有方法
        private void AddGravity()
        {
            _objRigidBody.AddForce(Vector3.down * _objGravity, ForceMode.Acceleration);
        }

        public void SetHeight(float targetHeight)
        {
            _objTransform.localScale = new Vector3(_objTransform.localScale.x, targetHeight, _objTransform.localScale.z);
        }

        #region Update
        public void FixedUpdate()
        {
            AddGravity(); //施加重力 
            GroundDetect(); //检测地面
        }
        #endregion

        #region Move
        public void ResetSpeed()
        {
            currentSpeed = _walkSpeed;
        }
        public Vector3 SetMoveDir(Vector3 targetDir)
        {
            return targetDir;
        }
        public void Move()
        {
            moveDir = SetMoveDir(_targetDir); //设置moveDir

            float playerRadius = .7f;
            float moveDistance = currentSpeed * Time.deltaTime;
            Vector3 capsuleOffset = Vector3.up * _objTransform.localScale.y * 0.5f;
            bool canMove = !Physics.CapsuleCast(_objTransform.position - capsuleOffset, _objTransform.position + capsuleOffset, playerRadius, moveDir, moveDistance);
            isMoving = moveDir != Vector3.zero;
            if (canMove)
            {
                _objTransform.position += moveDir.normalized * moveDistance;
            }
            else
            {
                Vector3 moveDirXNormalized = new Vector3(moveDir.x, 0, 0).normalized;
                //检测x轴是否能动
                canMove = !Physics.CapsuleCast(_objTransform.position - capsuleOffset, _objTransform.position + capsuleOffset, playerRadius, moveDirXNormalized, moveDistance);
                if (canMove)
                {
                    //如果只能动x轴
                    _objTransform.position += moveDirXNormalized * moveDistance;
                }
                else
                {
                    //如果x轴不能动，检测z轴
                    Vector3 moveDirZNormalized = new Vector3(0, 0, moveDir.z).normalized;
                    canMove = !Physics.CapsuleCast(_objTransform.position - capsuleOffset, _objTransform.position + capsuleOffset, playerRadius, moveDirZNormalized, moveDistance);
                    if (canMove)
                    {
                        //如果只能动z轴
                        _objTransform.position += moveDirZNormalized * moveDistance;
                    }
                    else
                    {
                        //什么都不处理
                    }
                }
            }

        }
        public void Sprint(bool isAllowed)
        {
            if (isAllowed)
            {
                currentSpeed = _sprintSpeed;
            }
            else
                ResetSpeed();
        }

        public bool GetIfMoving() //检测是否正在移动 
        {
            return isMoving;
        }
        #endregion
     
        #region GroundCheck
        //ground&slope check
        private RaycastHit _hit;
        [SerializeField]
        private bool _isOnGround = true;
        private bool _isOnSlope = false;
        private float _maxSlopeAngle = 45f;

        public void GroundDetect()
        {
            _isOnGround = Physics.Raycast(ObjTransform.position, Vector3.down, out _hit, .8f + ObjHeight * 0.5f);
            if (_isOnGround)
            {
                float angle = Vector3.Angle(Vector3.up, _hit.normal);
                if (angle < _maxSlopeAngle && angle != 0)
                {
                    _isOnSlope = true;
                }
                else
                {
                    _isOnSlope = false;
                }
            }
            else
            {
                _isOnSlope = false;
            }
            _readToJump = _isOnGround;
        }
        #endregion
        #region Crouch
        //crouch
        private float _crouchSpeed = 3f;
        private float _crouchHeight = .5f;
        public void StartCrouch(bool isAllowed)
        {
            if (isAllowed)
            {
                _objRigidBody.AddForce(Vector3.down * 10f, ForceMode.Impulse);
                SetHeight(_crouchHeight);
                currentSpeed = _crouchSpeed;
            }
        }
        public void StopCrouch(bool isAllowed)
        {
            if (isAllowed)
            {
                _objTransform.localScale = new Vector3(_objTransform.localScale.x, _objHeight, _objTransform.localScale.z);
                if (IsOnGround)
                {
                    currentSpeed = _walkSpeed;
                }
            }
        }

        #endregion
        #region Jump
        //jump 
        private bool _isJump = false;
        private bool _readToJump = true;
        private float _airSpeed = 4f;
        private float _jumpForce = 5f;
        private float _jumpCooldown = .5f;
        public void Jump(bool isAllowed)
        {
            if(isAllowed&& _readToJump && !_isJump)
            {
                currentSpeed = _airSpeed;
                _objRigidBody.velocity = new Vector3(_objRigidBody.velocity.x, 0, _objRigidBody.velocity.z); //重置y轴速度
                _objRigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                _readToJump = false;
                _isJump = true;
            }
        }
        public IEnumerator AfterJump() //跳跃冷却
        {
            yield return new WaitForSeconds(_jumpCooldown);
            _isJump = false;
        }
        #endregion
        #region Slide
        //slide 
        private float _slideForce = 500f;
        private float _slideTime = .5f;
        private float _slideHeight = .6f;
        private float slideTimer;
        private bool _isSlide = false; //检测相关按键
        private bool _sliding = false; //检测是否正在滑铲

        public void Slide(bool isAllowed)
        {
            if (isAllowed && !_sliding)
            {
                SetHeight(_slideHeight);
                _objRigidBody.AddForce(Vector3.down * 10f, ForceMode.Impulse);
                _sliding = true;
            }
        }
        public IEnumerator SlideDone()
        {
            _objRigidBody.AddForce(moveDir.normalized * _slideForce, ForceMode.Force);
            yield return new WaitUntil(() => //持续时长
            {

                slideTimer += Time.deltaTime;
                return _isJump || slideTimer > _slideTime;
            });
            SetHeight(_objHeight); //恢复原高度
            _sliding = false;
            slideTimer = 0;
        }
        #endregion
    }
}

