using UnityEngine;

public delegate void PlayerJoined(string playerName);


public sealed class LobbyManager : Photon.PunBehaviour
{

    #region Events
    public static event PlayerJoined OnPlayerJoined;
    #endregion

    private PhotonView myPhotonView;


    private void Awake() 
    {
        
    }
    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("JoinRandom");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }
}
