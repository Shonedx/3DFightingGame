using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeMotion : MonoBehaviour
{
    public LayerMask SlopeMask;
    public float maxSlopeAngle=45f;
    public bool isOnSlope = false;
    [Header("向量")]
    public RaycastHit slopeHit; //斜坡斜率
    public Vector3 slopeDir; //moveDir投影到斜坡的向量

    private MovementManager mM;
    void Start()
    {
        mM = GetComponent<MovementManager>();
        if (mM == null)
        {
            Debug.LogError("MovementManager组件未找到！");
        }
    }
    public void CheckSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 0.5f * mM.playerHeight + 0.8f, SlopeMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            isOnSlope = (bool)(angle < maxSlopeAngle && angle != 0);
        }
        else
            isOnSlope = false;
    }

    public void SetSlopeDir(Vector3 moveDir)
    {
        slopeDir= Vector3.ProjectOnPlane(moveDir.normalized, slopeHit.normal);
    }
}
