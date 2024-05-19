using UnityEngine;
using Map = GameSettings.Map;
using ExitGames.Client.Photon;
public class GameManager : Photon.PunBehaviour
{
    public static GameManager Instance { get; private set;}
    private byte[,] _map = new byte[Map.Rows, Map.Columns];
    
    // Y = 90 right
    // Y = -90 left
    // Y = 0 forward
    // Y = 180 backward

    [SerializeField] GameObject arrow;
    private void Awake() {
        if(Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
    private void Start()
    {
        arrow.transform.Rotate(90, 0 ,0);
        for (int row = 0; row < _map.GetLength(0); row++)
        {
            for (int column = 0; column < _map.GetLength(1); column++)
            {
                _map[row, column] = 1;
            }
        }


        
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

    public override void OnLeftRoom()
    {
        //SceneManager.LoadScene("PunBasics-Launcher");
    }

    #endregion
}
