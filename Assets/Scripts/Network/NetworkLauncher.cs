using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    // EDITOR FIELDS
    [SerializeField]
    [Tooltip("The UI panel where the user enters the nickname and connects to the game")]
    private GameObject controlPanel;
    [SerializeField]
    [Tooltip("The UI panel where the connection progress is displayed")]
    private GameObject progressLabel;

    // PRIVATE FIELDS
    private bool isConnecting;

    // PUBLIC CONSTANTS
    public const string gameVersion = "0.0";
    public const byte maxPlayersPerRoom = 2;

    // INITIALIZE
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        EnableConnectingControls(true);
    }

    // For now, just join a random room
    public void Connect()
    {
        EnableConnectingControls(false);

        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void EnableConnectingControls(bool enable)
    {
        controlPanel.SetActive(enable);
        progressLabel.SetActive(!enable);
    }

    // CALLBACKS
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() called by PUN");

        if(isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() called with reason {0}", cause);
        EnableConnectingControls(true);
        isConnecting = false;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room.  Creating new room...");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom } );
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined a room");

        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'NetworkBattleground' ");

            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("NetworkBattleground");
        }
    }
}
