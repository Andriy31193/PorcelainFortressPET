using System;
using UnityEngine;
using Map = GameSettings.Map;

public delegate void OnPlayerMoved(IPlayer player, byte x, byte z);

[RequireComponent(typeof(MazeBuilder))]
public class GameManager : Photon.PunBehaviour
{
    public GameStatus GameStatus
    {
        get => _gameStatus;
        private set
        {
            _gameStatus = value;
            OnGameStatusChanged?.Invoke(_gameStatus);
        }
    }
    public static event Action<GameStatus> OnGameStatusChanged;
    public static event Action<byte[,]> OnMazeDataReceived;
    public static OnPlayerMoved OnPlayerMoved { get; private set; }

    private byte[,] _maze, _defaultMaze;

    // Temporary solution for player trails //
    private byte[,] _trail;
    private GameStatus _gameStatus;

    private MazeBuilder _mazeBuilder;
    private BoardUIManager _boardUIManager;
    private GameUIControl _gameUIControl;
    private EntitiesCollection _entitiesCollection;
    private IPlayer _localPlayer;



    private void Awake()
    {
        DIContainer.Register(this);
    }
    private void Start()
    {
        _boardUIManager = DIContainer.Resolve<BoardUIManager>();
        _mazeBuilder = DIContainer.Resolve<MazeBuilder>();
        _entitiesCollection = DIContainer.Resolve<EntitiesCollection>();
        _gameUIControl = DIContainer.Resolve<GameUIControl>();
        _localPlayer = DIContainer.Resolve<IPlayer>();

        OnPlayerMoved += OnIPlayerMovedEvent;

        ClockManager.OnClockReady += OnClockReady;

        BoardUIManager.OnUIEntityChange += OnUIEntityChangeEvent;

        GameUIControl.OnStartButtonPressed += OnUIStartPressed;
        GameUIControl.OnResetButtonPressed += OnUIResetPressed;
        GameUIControl.OnStopButtonPressed += OnUIStopPressed;
    }

    public void SetMazeEntity(byte entity, byte row, byte column)
    {
        _maze[row, column] = entity;
        VisualizeMaze();
    }
    public bool TryMovePlayerOnMaze(byte row, byte column)
    {
        if (row >= _maze.GetLength(0) || column >= _maze.GetLength(1))
            return false;

        if ((EntityType)_maze[row, column] == EntityType.Wall)
            return false;

        return true;
    }
    public void HandleCreation(byte row, byte column, EntityType type)
    {
        _mazeBuilder.ModifyEntitiy(row, column, type);
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

        _trail[x, z] = 255;
        VisualizeMaze();
    }
    public void OnUIEntityChangeEvent(byte entity, byte row, byte column)
    {
        _mazeBuilder.ModifyEntitiy(row, column, (EntityType)entity);
        SetMazeEntity(entity, row, column);
    }
    public void OnUIStartPressed()
    {
        _localPlayer.StartMovement();
    }
    public void OnClockReady()
    {
        GameStatus = GameStatus.Playing;
    }
    public void OnUIResetPressed()
    {
        _localPlayer.ResetMovement();

        _maze = new byte[_defaultMaze.GetLength(0), _defaultMaze.GetLength(1)];
        _trail = new byte[_defaultMaze.GetLength(0), _defaultMaze.GetLength(1)];
        Buffer.BlockCopy(_defaultMaze, 0, _maze, 0, _defaultMaze.Length * sizeof(byte));

        OnMazeDataReceived?.Invoke(_maze);
        VisualizeMaze();
    }
    private void VisualizeMaze()
    {
        int rows = _maze.GetLength(0);
        int columns = _maze.GetLength(1);

        byte[,] _mazeToVisualize = new byte[rows, columns];

        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                if (_trail[row, column] == 255)
                {
                    _mazeToVisualize[row, column] = _trail[row, column];
                }
                else _mazeToVisualize[row, column] = _maze[row, column];
            }
        }
        _boardUIManager.RefreshBoard(_mazeToVisualize, true);
    }
    public void OnUIStopPressed()
    {
        _localPlayer.StopMovement();
    }
    #region Mono
    private void OnDestroy()
    {
        OnPlayerMoved -= OnIPlayerMovedEvent;

        ClockManager.OnClockReady -= OnClockReady;
        BoardUIManager.OnUIEntityChange -= OnUIEntityChangeEvent;
        GameUIControl.OnStartButtonPressed -= OnUIStartPressed;
        GameUIControl.OnResetButtonPressed -= OnUIResetPressed;
        GameUIControl.OnStopButtonPressed -= OnUIStopPressed;
    }
    #endregion
    #region Photon Messages
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            if (PhotonNetwork.room.PlayerCount > 1)
                _mazeBuilder.Generate(OnMazeGenerated);

        }
    }
    public override void OnJoinedRoom()
    {
        // if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        // {
        //     _mazeBuilder.Generate(OnMazeGenerated);
        // }
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
        _trail = new byte[Map.Rows, Map.Columns];
        _defaultMaze = new byte[Map.Rows, Map.Columns];

        Buffer.BlockCopy(data, 0, _maze, 0, data.Length);
        Buffer.BlockCopy(data, 0, _defaultMaze, 0, data.Length);

        OnMazeDataReceived?.Invoke(_defaultMaze);

        GameStatus = GameStatus.Ready;
    }
    #endregion
}
