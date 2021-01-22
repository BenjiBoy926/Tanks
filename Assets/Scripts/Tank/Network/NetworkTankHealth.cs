using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class NetworkTankHealth : TankHealth, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(m_CurrentHealth);
            stream.SendNext(m_Dead);
        }
        else
        {
            m_CurrentHealth = (float)stream.ReceiveNext();
            m_Dead = (bool)stream.ReceiveNext();
        }
    }
}
