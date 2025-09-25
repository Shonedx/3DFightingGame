using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeMotion : MonoBehaviour
{
    public LayerMask SlopeMask;
    public float maxSlopeAngle=45f;
    public bool isOnSlope = false;
    [Header("����")]
    public RaycastHit slopeHit; //б��б��
    public Vector3 slopeDir; //moveDirͶӰ��б�µ�����

    private MovementManager mM;
    void Start()
    {
        mM = GetComponent<MovementManager>();
        if (mM == null)
        {
            Debug.LogError("MovementManager���δ�ҵ���");
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
