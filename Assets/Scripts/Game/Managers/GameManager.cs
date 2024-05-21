using System;
using UnityEngine;
using Map = GameSettings.Map;
public delegate void OnPlayerMoved(IPlayer player);

[RequireComponent(typeof(MazeBuilder))]

public class GameManager : Photon.PunBehaviour
{
    public static GameManager Instance { get; private set; }

    public static OnPlayerMoved OnPlayerMoved { get; set; }

    // Y = 90 right
    // Y = -90 left
    // Y = 0 forward
    // Y = 180 backward

    private MazeBuilder _mazeBuilder;
    private byte[,] _maze;
    private void Awake()
    {
        OnPlayerMoved += OnIPlayerMovedEvent;
        BoardUIBuilder.OnUIEntityChange += OnUIEntityChangeEvent;

        _mazeBuilder = GetComponent<MazeBuilder>();

        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
    private void Start()
    {

    }
    public void SetMazeEntity(byte entity, byte row, byte column)
    {
        _maze[row, column] = entity;
        BoardUIBuilder.Instance.RefreshBoard(_maze);
    }
    public bool TryMovePlayerOnMaze(byte row, byte column)
    {

        if ((EntityType)_maze[row, column] == EntityType.Wall)
            return false;

        SetMazeEntity(255, row, column);

        return true;
    }
    private void OnIPlayerMovedEvent(IPlayer player)
    {
        Debug.Log("moved");
    }
    public void HandleCreation(Vector3 position)
    {
        GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.position = position;
    }
    public void OnMazeGenerated(byte[] data)
    {
        photonView.RPC(nameof(OnReceiveMazeData), PhotonTargets.AllBuffered, data);
    }
    public void OnUIEntityChangeEvent(byte entity, byte row, byte column)
    {
        MazeBuilder.Instance.ModifyEntitiy(row, column, EntityType.Void);
        SetMazeEntity(0, row, column);
    }
    #region Mono
    private void OnDestroy() 
    {
        BoardUIBuilder.OnUIEntityChange -= OnUIEntityChangeEvent;
    }
    #endregion
    #region Photon Messages
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            _mazeBuilder.Generate(OnMazeGenerated);
        }
    }
    #endregion
    #region PunRPC
    [PunRPC]
    public void OnReceiveMazeData(byte[] data)
    {
        if (data == null)
        {
            Debug.LogError("Error occured while receiving OnReceiveMazeData");
            return;
        }

        _maze = new byte[Map.Rows, Map.Columns];

        Buffer.BlockCopy(data, 0, _maze, 0, data.Length);

        _mazeBuilder.Build3DMaze(_maze);
        BoardUIBuilder.Instance.Build(_maze);

        PlayerController.Instance.StartMovement();
    }
    #endregion
}
