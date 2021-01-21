using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class NetworkGameManager : GameManager, IPunObservable
{
    // CALLBACKS
    protected override void Start()
    {
        base.Start();

        // If local player is not set up yet, set them up with a tank
        if (PhotonNetwork.LocalPlayer.TagObject == null)
        {
            Debug.Log("Creating tank for the local player. Actor Number: " + PhotonNetwork.LocalPlayer.ActorNumber);
            SetupNetworkPlayer(PhotonNetwork.LocalPlayer);
        }

        // If max players reached, start gameplay
        if (PhotonNetwork.CurrentRoom.PlayerCount == NetworkLauncher.maxPlayersPerRoom)
        {
            StartCoroutine("WaitForGameToBegin");
        }
    }
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

        if(PhotonNetwork.IsMasterClient)
        {
            LoadArena();
        }
    }
    // Send the current local player over the network
    // When a local player is received, setup the tag object of that player
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsReading)
        {
            Player theirPlayer = (Player)stream.ReceiveNext();

            Debug.Log("Actor #" + PhotonNetwork.LocalPlayer.ActorNumber + " is receiving information from Actor #" + theirPlayer.ActorNumber);

            Player myVersionOfTheirPlayer = PhotonNetwork.PlayerList.First(x => x.ActorNumber == theirPlayer.ActorNumber);
            myVersionOfTheirPlayer.TagObject = theirPlayer.TagObject;
        }
        else
        {
            Debug.Log("Actor #" + PhotonNetwork.LocalPlayer.ActorNumber + " is sending their information");
            stream.SendNext(PhotonNetwork.LocalPlayer);
        }
    }
    // Get tank instances by getting all player tag objects
    protected override GameObject[] GetTankInstances()
    {
        Player[] playerList = PhotonNetwork.PlayerList;
        GameObject[] tankInstances = new GameObject[playerList.Length];

        for (int i = 0; i < playerList.Length; i++)
        {
            tankInstances[i] = (GameObject)playerList[i].TagObject;
        }

        return tankInstances;
    }

    // PRIVATE METHODS
    System.Collections.IEnumerator WaitForGameToBegin()
    {
        Debug.Log("Waiting until all player tag objects have been received over the network");

        yield return new WaitUntil(() =>
        {
            Player[] playerList = PhotonNetwork.PlayerList;
            return playerList.Count(x => x.TagObject != null) == playerList.Length;
        });

        Debug.Log("Client is ready to start the game!");
        GameplayBegin();
    }
    private void LoadArena()
    {
        Debug.Log("Master client loading the arena");
        PhotonNetwork.LoadLevel("NetworkBattleground");
    }
    private void SetupNetworkPlayer(Player player)
    {
        byte playerIndex = (byte)(PhotonNetwork.CurrentRoom.PlayerCount - 1);

        // Get the correct spawn point and instantiate the network player
        Transform spawn = m_Tanks[playerIndex].m_SpawnPoint;
        GameObject instance = PhotonNetwork.Instantiate(m_TankPrefab.name, spawn.position, spawn.rotation, 0);
        DontDestroyOnLoad(instance);

        // Tag the player with the instance of the tank that they control
        player.TagObject = instance;
    }
}
