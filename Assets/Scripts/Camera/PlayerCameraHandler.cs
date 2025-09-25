using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
[DefaultExecutionOrder(-1)] //确保摄像机脚本在玩家控制脚本之后执行
public class PlayerCameraHandler : MonoBehaviour
{
    [Header("人称设置")]
    [SerializeField] private GameObject firstPersonCameras;
    [SerializeField] private GameObject thirdPersonCameras;
    private bool isFirstPerson = true;
    [Header("摄像机设置")]
    [Tooltip("第一人称相机")]
    [SerializeField] private CinemachineVirtualCamera firstPersonVirtualCamera;
    [Tooltip("第一人称瞄准相机")]
    [SerializeField] private CinemachineVirtualCamera firstPersonAimVirtualCamera;
    [Tooltip("第三人称相机")]
    [SerializeField] private CinemachineVirtualCamera thirdPersonVirtualCamera;
    [Tooltip("第三人称瞄准相机")]
    [SerializeField] private CinemachineVirtualCamera thirdPersonAimVirtualCamera;
    [Header("玩家")]
    [SerializeField] private GameObject player;
    [Header("视觉中心点")]
    [SerializeField] private GameObject visualCenterPoint;
    [Header("游戏输入设置")]
    [SerializeField] private GameInput gameInput;

    #region CameraParameters
    private bool isAiming = false; //是否瞄准
    [Header("相机旋转参数")]
    [SerializeField] private float rotateXSensitivity = .2f;
    [SerializeField] private float rotateYSensitivity = .2f;
    private Vector2 lookDir=Vector2.zero; //视角方向
    private float rotationX;
    private float rotationY;
    private float minViewYAngle = -70f;
    private float maxViewYangle = 70f;
    #endregion
    public enum CameraState
    {
        firstPersonCameraState = 0,
        thirdPersonCameraState,
    }
    [field: SerializeField] public CameraState currentCameraState { get; private set; } = CameraState.firstPersonCameraState;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; //锁定鼠标光标
        UpdatePersonCameraState(); //初始化人称状态
    }
    #region Update
    private void Update()
    {
        UpdatePersonCameraState(); //
        UpdateAimingState(); //更新瞄准状态
        UpdateLookDir(); //更新视角方向
    }

    private void UpdateLookDir()
    {
        lookDir = gameInput.GetLookDirection();
        rotationX += lookDir.x * rotateXSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY - lookDir.y * rotateYSensitivity * Time.deltaTime, minViewYAngle, maxViewYangle);
    }
 
    private void UpdateAimingState() //更新瞄准状态 瞄准状态的虚拟相机优先级比普通相机高
    {
        isAiming=gameInput.GetAimBool();
        switch (currentCameraState)
        {
            case CameraState.firstPersonCameraState:
                firstPersonAimVirtualCamera.gameObject.SetActive(isAiming);
                thirdPersonAimVirtualCamera.gameObject.SetActive(false);
                break;
            case CameraState.thirdPersonCameraState:
                firstPersonAimVirtualCamera.gameObject.SetActive(false);
                thirdPersonAimVirtualCamera.gameObject.SetActive(isAiming);
                break;
            default:
                break;
        }
    }
    private void UpdatePersonCameraState() //用来设置第一人称还是第三人称
    {
        bool doSwitch = gameInput.GetPersonViewSwitchBool();
        if (doSwitch)
        {
            isFirstPerson = !isFirstPerson;
            currentCameraState = isFirstPerson ? CameraState.firstPersonCameraState : CameraState.thirdPersonCameraState;
        } 
        switch (currentCameraState)
        {
            case CameraState.firstPersonCameraState:
                firstPersonCameras.SetActive(true);
                thirdPersonCameras.SetActive(false);
                break;
            case CameraState.thirdPersonCameraState:
                firstPersonCameras.SetActive(false);
                thirdPersonCameras.SetActive(true);
                break;
            default:
                break;
        }
    }
#endregion 
    #region LateUpdate
    private void LateUpdate()
    {
        CameraRotationHandler(); //更新相机旋转
    }
    private void CameraRotationHandler()
    {
        Quaternion cameraRotation = Quaternion.Euler(rotationY, rotationX, 0); //相机旋转角度
        Quaternion playerRotation = Quaternion.Euler(0, rotationX, 0); //玩家旋转角度
        visualCenterPoint.transform.rotation = cameraRotation;
        player.transform.rotation = playerRotation;
    }
    #endregion
}
