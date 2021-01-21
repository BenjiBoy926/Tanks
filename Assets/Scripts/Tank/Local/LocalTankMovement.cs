using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalTankMovement : TankMovement
{
    protected override void SetInputValues()
    {
        m_MovementInputValue = Input.GetAxis("Vertical" + m_PlayerNumber);
        m_TurnInputValue = Input.GetAxis("Horizontal" + m_PlayerNumber);
    }
}
