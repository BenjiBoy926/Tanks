using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class NetworkShellExplosion : ShellExplosion
{
    protected override void OnTriggerEnter(Collider other)
    {
        if(photonView.IsMine)
        {
            base.OnTriggerEnter(other);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
