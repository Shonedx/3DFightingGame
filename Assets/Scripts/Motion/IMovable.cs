using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motion.Interface
{
    public interface IMovable
    {
        void Move(Vector3 direction);
        Vector3 SetMoveDir(Vector3 direction);
        void SetMovementEnabled(bool enabled);
        void SetMovementSpeed(float speed);
        Vector3 MoveDir { get; }
        float MoveSpeed { get;}
        bool IfEnableGameInput { get; }
        bool IsMoving { get; }
    }
}

