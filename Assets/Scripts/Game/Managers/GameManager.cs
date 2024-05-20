using UnityEngine;
[RequireComponent(typeof(MazeGenerator))]
public class GameManager : Photon.PunBehaviour
{
    public static GameManager Instance { get; private set; }

    // Y = 90 right
    // Y = -90 left
    // Y = 0 forward
    // Y = 180 backward

    private MazeGenerator _generator;
    private void Awake()
    {
        _generator = GetComponent<MazeGenerator>();

        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
    private void Start()
    {

    }

    public void HandleCreation(Vector3 position)
    {
        GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.position = position;
    }

    #region Photon Messages

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
            //LoadArena();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
            //LoadArena();
        }
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            _generator.Generate();
        }
    }
    public override void OnLeftRoom()
    {
        //SceneManager.LoadScene("PunBasics-Launcher");
    }

    #endregion
}
