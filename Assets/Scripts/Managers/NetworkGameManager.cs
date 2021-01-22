using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class NetworkGameManager : GameManager, IPunObservable
{
    public Text connectingText;   

    // PROPERTIES
    public static int localPlayerIndex
    {
        get
        {
            return PhotonNetwork.LocalPlayer.ActorNumber - 1;
        }
    }
    public static GameObject localInstance
    {
        get
        {
            return (GameObject)PhotonNetwork.LocalPlayer.TagObject;
        }
        private set
        {
            PhotonNetwork.LocalPlayer.TagObject = value;
        }
    }

    public static bool gameplayReady
    {
        get
        {
            return PhotonNetwork.CurrentRoom.PlayerCount == NetworkLauncher.maxPlayersPerRoom;
        }
    }

    // PUBLIC INTERFACE
    public static int PlayerIndex(Player player)
    {
        return player.ActorNumber - 1;
    }

    // UNITY CALLBACKS
    protected override void Start()
    {
        base.Start();

        // If tank is not yet set up, set it up
        if (localInstance == null)
        {
            Debug.Log("Creating tank for the local player. Player #: " + PhotonNetwork.LocalPlayer.ActorNumber);
            SetupLocalTankInstance();
        }

        // Setup the targets for the camera to follow
        SetupCameraTargets();

        // If max players reached, start gameplay
        if (gameplayReady)
        {
            connectingText.text = "";
            StartCoroutine("WaitForGameToBegin");
        }
        else
        {
            m_MessageText.text = "";
            connectingText.text = "Waiting for an opponent to connect...";
        }
    }
    // If gameplay is not ready and the tank disables, reset it
    private void Update()
    {
        if(!gameplayReady && !localInstance.activeInHierarchy)
        {
            m_Tanks[localPlayerIndex].Reset();
        }
    }

    // PUN CALLBACKS
    // ENTERED/EXITED ROOM
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered the room!");

        if(PhotonNetwork.IsMasterClient)
        {
            LoadArena();
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left the room!");

        if (PhotonNetwork.IsMasterClient)
        {
            LoadArena();
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log("Player #" + localPlayerIndex + " is sending their tank managers");
            stream.SendNext(m_Tanks);
        }
        else
        {
            Debug.Log("Player #" + localPlayerIndex + " is receiving tank managers from " + PlayerIndex(info.Sender));
            m_Tanks = (TankManager[])stream.ReceiveNext();
        }
    }

    // OVERRIDES
    // Get tank instances by using the tag object
    protected override GameObject[] GetTankInstances()
    {
        return PhotonNetwork.PlayerList.Select(x => (GameObject)x.TagObject).ToArray();
    }

    // PRIVATE METHODS
    System.Collections.IEnumerator WaitForGameToBegin()
    {
        Debug.Log("Waiting until all player tanks have been found");

        yield return new WaitUntil(() =>
        {
            GameObject[] instances = GetTankInstances();
            return instances.Count(x => x != null) == m_Tanks.Length;
        });

        Debug.Log("Client is ready to start the game!");
        GameplayBegin();
    }
    private void LoadArena()
    {
        Debug.Log("Master client loading the arena");
        PhotonNetwork.LoadLevel("NetworkBattleground");
    }
    private void SetupLocalTankInstance()
    {
        // Get the correct spawn point and instantiate the network player
        Transform spawn = m_Tanks[localPlayerIndex].m_SpawnPoint;
        localInstance = PhotonNetwork.Instantiate(m_TankPrefab.name, spawn.position, spawn.rotation, 0);
        DontDestroyOnLoad(localInstance);

        // Setup the current tank instance
        m_Tanks[localPlayerIndex].Setup(localInstance, localPlayerIndex + 1);
    }

    // NOPE: this is called on the object that is actually instantiated!  It's not called on the object that DOES the instantiating
    //public void OnPhotonInstantiate(PhotonMessageInfo info)
    //{
    //    Debug.Log("Player #" + (localPlayerIndex + 1) + " received instantiate message from Player #" + info.Sender.ActorNumber);
    //    tankInstances[info.Sender.ActorNumber - 1] = (GameObject)info.Sender.TagObject;
    //}

    // NOPE: for some reason this function is not getting called
    // This is only used to sync MY OWN DATA between clients - it can't be used to send my own information to other clients
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    // Send local player index with the corresponding instance
    //    if (stream.IsWriting)
    //    {
    //        Debug.Log("Player #" + localPlayerIndex + " is sending their tank instance");
    //        stream.SendNext(localPlayerIndex);
    //        stream.SendNext(localInstance);
    //    }
    //    // Receive the other index and instance, and store it in the list
    //    else
    //    {
    //        int otherIndex = (int)stream.ReceiveNext();
    //        GameObject otherInstance = (GameObject)stream.ReceiveNext();

    //        Debug.Log("Player #" + localPlayerIndex + 
    //            " is receiving tank instance from Player #" + otherIndex);

    //        tankInstances[otherIndex] = otherInstance;
    //    }
    //}

    // Send this local player tank instance to all other clients who need a reference to it
    //private void SynchronizeLocalTankInstance()
    //{
    //    photonView.RPC("RecieveTankInstance", RpcTarget.Others, localPlayerIndex, localInstance);
    //}

    // NOPE: this does not work because Photon cannot send a GameObject by RPC
    // Receive an actor number of a player as well as the tank instance associated with it
    //[PunRPC]
    //public void RecieveTankInstance(int otherPlayerIndex, object otherInstance)
    //{
    //    Debug.Log("Player #" + localPlayerIndex + " recieved RPC call from Player #" + otherPlayerIndex);
    //    tankInstances[otherPlayerIndex] = (GameObject)otherInstance;
    //}
}
