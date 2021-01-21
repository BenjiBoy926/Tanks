using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class NetworkTankShooting : TankShooting
{
    protected override bool GetButtonDown()
    {
        return Input.GetButtonDown("Fire") && (photonView.IsMine || !PhotonNetwork.IsConnected);
    }
    protected override bool GetButton()
    {
        return Input.GetButton("Fire") && (photonView.IsMine || !PhotonNetwork.IsConnected);
    }
    protected override bool GetButtonUp()
    {
        return Input.GetButtonUp("Fire") && (photonView.IsMine || !PhotonNetwork.IsConnected);
    }
    protected override Rigidbody InstantiateShell()
    {
        GameObject instance;
        if (PhotonNetwork.IsConnected)
        {
            instance = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation);
        }
        else
        {
            instance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as GameObject;

        }
        return instance.GetComponent<Rigidbody>();
    }
}
