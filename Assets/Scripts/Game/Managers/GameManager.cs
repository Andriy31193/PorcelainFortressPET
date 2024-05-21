using System;
using UnityEditor.UI;
using UnityEngine;
using Map = GameSettings.Map;

public delegate void OnPlayerMoved(IPlayer player, byte x, byte z);

[RequireComponent(typeof(MazeBuilder))]
public class GameManager : Photon.PunBehaviour
{
    public static event Action<byte[,]> OnMazeDataReceived;
    public static OnPlayerMoved OnPlayerMoved { get; private set;}

    private byte[,] _maze;

    private MazeBuilder _mazeBuilder;
    private BoardUIManager _boardUIBuilder;
    private GameUIControl _gameUIControl;
    private EntitiesCollection _entitiesCollection;
    private IPlayer _localPlayer;



    private void Awake()
    {
        DIContainer.Register(this);
    }
    private void Start()
    {
        _boardUIBuilder = DIContainer.Resolve<BoardUIManager>();
        _mazeBuilder = DIContainer.Resolve<MazeBuilder>();
        _entitiesCollection = DIContainer.Resolve<EntitiesCollection>();
        _gameUIControl = DIContainer.Resolve<GameUIControl>();
        _localPlayer = DIContainer.Resolve<IPlayer>();

        OnPlayerMoved += OnIPlayerMovedEvent;

        BoardUIManager.OnUIEntityChange += OnUIEntityChangeEvent;
        GameUIControl.OnStartButtonPressed += OnUIStartPressed;
        GameUIControl.OnStopButtonPressed += OnUIStopPressed;
    }
    public void SetMazeEntity(byte entity, byte row, byte column)
    {
        _maze[row, column] = entity;
        _boardUIBuilder.RefreshBoard(_maze);
    }
    public bool TryMovePlayerOnMaze(byte row, byte column)
    {

        if ((EntityType)_maze[row, column] == EntityType.Wall)
            return false;

        return true;
    }
    public void HandleCreation(EntityType type, byte row, byte column, Vector3 position)
    {
        _mazeBuilder.HandleCreation(type, position);
        SetMazeEntity((byte)type, row, column);
    }
    private void OnMazeGenerated(byte[] data)
    {
        photonView.RPC(nameof(OnReceiveMazeData), PhotonTargets.AllBuffered, data);
    }
    private void OnIPlayerMovedEvent(IPlayer player, byte x, byte z)
    {
        var entity = _entitiesCollection.GetEntitity((EntityType)_maze[x, z]);

        if (entity != null)
            entity.Affect(player);

        SetMazeEntity(255, x, z);
    }
    public void OnUIEntityChangeEvent(byte entity, byte row, byte column)
    {
        _mazeBuilder.ModifyEntitiy(row, column, EntityType.Void);
        SetMazeEntity(0, row, column);
    }
    public void OnUIStartPressed()
    {
        _localPlayer.StartMovement();
    }
    public void OnUIStopPressed()
    {
        _localPlayer.StopMovement();
    }
    #region Mono
    private void OnDestroy()
    {
        BoardUIManager.OnUIEntityChange -= OnUIEntityChangeEvent;
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

        OnMazeDataReceived?.Invoke(_maze);
    }
    #endregion
}
