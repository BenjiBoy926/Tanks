using UnityEngine;
using System.Collections;

using Photon.Pun;

public class TankNetworking : MonoBehaviourPunCallbacks
{
    public static GameObject localPlayer = null;

    private void Awake()
    {
        if(photonView.IsMine)
        {
            localPlayer = gameObject;
        }
        DontDestroyOnLoad(gameObject);
    }
}
