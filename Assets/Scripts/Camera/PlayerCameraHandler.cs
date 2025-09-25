using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
[DefaultExecutionOrder(-1)] //ȷ��������ű�����ҿ��ƽű�֮��ִ��
public class PlayerCameraHandler : MonoBehaviour
{
    [Header("�˳�����")]
    [SerializeField] private GameObject firstPersonCameras;
    [SerializeField] private GameObject thirdPersonCameras;
    private bool isFirstPerson = true;
    [Header("���������")]
    [Tooltip("��һ�˳����")]
    [SerializeField] private CinemachineVirtualCamera firstPersonVirtualCamera;
    [Tooltip("��һ�˳���׼���")]
    [SerializeField] private CinemachineVirtualCamera firstPersonAimVirtualCamera;
    [Tooltip("�����˳����")]
    [SerializeField] private CinemachineVirtualCamera thirdPersonVirtualCamera;
    [Tooltip("�����˳���׼���")]
    [SerializeField] private CinemachineVirtualCamera thirdPersonAimVirtualCamera;
    [Header("���")]
    [SerializeField] private GameObject player;
    [Header("�Ӿ����ĵ�")]
    [SerializeField] private GameObject visualCenterPoint;
    [Header("��Ϸ��������")]
    [SerializeField] private GameInput gameInput;

    #region CameraParameters
    private bool isAiming = false; //�Ƿ���׼
    [Header("�����ת����")]
    [SerializeField] private float rotateXSensitivity = .2f;
    [SerializeField] private float rotateYSensitivity = .2f;
    private Vector2 lookDir=Vector2.zero; //�ӽǷ���
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
        Cursor.lockState = CursorLockMode.Locked; //���������
        UpdatePersonCameraState(); //��ʼ���˳�״̬
    }
    #region Update
    private void Update()
    {
        UpdatePersonCameraState(); //
        UpdateAimingState(); //������׼״̬
        UpdateLookDir(); //�����ӽǷ���
    }

    private void UpdateLookDir()
    {
        lookDir = gameInput.GetLookDirection();
        rotationX += lookDir.x * rotateXSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY - lookDir.y * rotateYSensitivity * Time.deltaTime, minViewYAngle, maxViewYangle);
    }
 
    private void UpdateAimingState() //������׼״̬ ��׼״̬������������ȼ�����ͨ�����
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
    private void UpdatePersonCameraState() //�������õ�һ�˳ƻ��ǵ����˳�
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
        CameraRotationHandler(); //���������ת
    }
    private void CameraRotationHandler()
    {
        Quaternion cameraRotation = Quaternion.Euler(rotationY, rotationX, 0); //�����ת�Ƕ�
        Quaternion playerRotation = Quaternion.Euler(0, rotationX, 0); //�����ת�Ƕ�
        visualCenterPoint.transform.rotation = cameraRotation;
        player.transform.rotation = playerRotation;
    }
    #endregion
}
