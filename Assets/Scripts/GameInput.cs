using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[DefaultExecutionOrder(-2)] //ȷ������ű��������ű�֮ǰִ��
public class GameInput : MonoBehaviour
{
    private InputSystem_Actions playerInputAction;

    private void Awake()
    {
        playerInputAction = new InputSystem_Actions();
        playerInputAction.Enable();
    }
    private void OnDisable()
    {
        playerInputAction.Disable();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public Vector2 GetMovementNormalized() //��ȡ��һ�����ƶ�����
    { 
        Vector2 inputVector2=playerInputAction.Player.Move.ReadValue<Vector2>();
        Debug.Log("input:" + inputVector2);
        return inputVector2.normalized;
    }
    public bool GetPersonViewSwitchBool() //ʶ���˳��л������Ƿ���
    {
        return playerInputAction.Player.SwitchLookSight.WasPressedThisFrame(); //�������´���;
    }
    public bool GetAimBool()
    {
        return playerInputAction.Player.Aim.IsPressed(); //ʶ�𰴼��Ƿ�ס
    }
    public Vector2 GetLookDirection()
    {
        Vector2 lookDir = playerInputAction.Player.Look.ReadValue<Vector2>();
        return lookDir;
    }
    public bool GetToJump()
    {
        return playerInputAction.Player.Jump.WasPressedThisFrame();
    }
    public bool GetCrouchPerformed()
    {
        return playerInputAction.Player.Crouch.WasPressedThisFrame();
    }
    public bool GetCrouchCanceled()
    {
        return playerInputAction.Player.Crouch.WasReleasedThisFrame();
    }
    public bool GetToSprint()
    {
        return playerInputAction.Player.Sprint.IsPressed();
    }
    public bool GetToShot()
    {
        return playerInputAction.Player.Attack.IsPressed(); //���԰�ס���
    }
    public bool GetToReload()
    {
        return playerInputAction.Player.Reload.WasPressedThisFrame();
    }
    public bool GetToSlide()
    {
        return playerInputAction.Player.Slide.WasPressedThisFrame();
    }
    //public void OnSwitchLookSight(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {
    //        camCtrlr.isFirstPerson = !camCtrlr.isFirstPerson;
    //    }
    //}
    //public void OnLook(InputAction.CallbackContext ctx)
    //{
    //    InputDevice device = ctx.control.device;
    //    camCtrlr.isUsingMouse = device is Mouse;


    //    camCtrlr.camRotateDelta = ctx.ReadValue<Vector2>();
    //}
    //public void OnReload(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed && shotMngr.leftBullets < shotMngr.bulletsMagazineCap && !shotMngr.isReloading)
    //        shotMngr.Reload();
    //}

    //private void AttackInput()
    //{
    //    if (shotMngr.allowButtonHold)
    //    {
    //        shotMngr.isShooting = attackAction.IsPressed();
    //    }
    //    else shotMngr.isShooting = attackAction.WasPerformedThisFrame();

    //    if (shotMngr.isShooting && shotMngr.readToShoot && !shotMngr.isReloading && shotMngr.leftBullets <= 0) shotMngr.Reload(); //�ӵ�����Զ�װ��
    //    if (shotMngr.isShooting && shotMngr.readToShoot && !shotMngr.isReloading && shotMngr.leftBullets > 0)
    //    {
    //        shotMngr.shotBullets = 0;
    //        shotMngr.readToShoot = false;
    //        shotMngr.Shoot();
    //    }
    //}

    //public void OnSlide(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed) //slide
    //    {
    //        slideMotion.Slide(movMngr.moveDir);
    //    }
    //}
    //public void OnCrouch(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {
    //        movMngr.StartCrouch();
    //    }
    //    else if (ctx.canceled)
    //    {
    //        movMngr.StopCrouch();
    //    }
    //}
    //public void OnJump(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed&& (movMngr.isOnGround || (slpMotion?.isOnSlope ?? false)) && !movMngr.isJump && movMngr.readToJump)
    //    {
    //        movMngr.Jump();
    //    }
    //}
    //public void OnSprint(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.control.IsPressed())
    //    {
    //        movMngr.currentSpeed = movMngr.sprintSpeed;
    //    }
    //    else if (ctx.canceled && !slideMotion.isSlide)
    //    {
    //        movMngr.currentSpeed = movMngr.walkSpeed;
    //    }
    //}
}
