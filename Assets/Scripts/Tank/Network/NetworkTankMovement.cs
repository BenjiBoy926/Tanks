using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class NetworkTankMovement : TankMovement
{
    protected override void SetInputValues()
    {
        if(photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            m_MovementInputValue = Input.GetAxis("Vertical");
            m_TurnInputValue = Input.GetAxis("Horizontal");
        }
    }
}
