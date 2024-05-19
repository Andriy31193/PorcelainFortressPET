using UnityEngine;

public delegate void PlayerJoined(string playerName);


public sealed class LobbyManager : Photon.PunBehaviour
{
    public static LobbyManager Instance { get; private set; }

    #region Events
    public static event PlayerJoined OnPlayerJoined;
    #endregion
    
    private PhotonView myPhotonView;


    private void Awake() 
    {
        if(Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
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
        // when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
        GameObject monster = PhotonNetwork.Instantiate("monsterprefab", Vector3.zero, Quaternion.identity, 0);
        monster.GetComponent<myThirdPersonController>().isControllable = true;
        myPhotonView = monster.GetComponent<PhotonView>();

        // Raise the player joined event with the player's name
        if (OnPlayerJoined != null)
        {
            OnPlayerJoined(PhotonNetwork.playerName);
        }
    }

    public void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (PhotonNetwork.inRoom)
        {
            bool shoutMarco = GameLogic.playerWhoIsIt == PhotonNetwork.player.ID;

            if (shoutMarco && GUILayout.Button("Marco!"))
            {
                myPhotonView.RPC("Marco", PhotonTargets.All);
            }
            if (!shoutMarco && GUILayout.Button("Polo!"))
            {
                myPhotonView.RPC("Polo", PhotonTargets.All);
            }
        }
    }
}
