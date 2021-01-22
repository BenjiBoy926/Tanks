using UnityEngine;
using System.Collections;

using Photon.Pun;

public class TankNetworking : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("Player #" + NetworkGameManager.localPlayerIndex +
            " received OnPhotonInstantiate callback from Player #" +
            NetworkGameManager.PlayerIndex(info.Sender));
        info.Sender.TagObject = gameObject;
    }
}
