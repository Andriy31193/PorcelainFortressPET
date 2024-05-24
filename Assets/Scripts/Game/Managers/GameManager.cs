using System;
using UnityEngine;
using Map = GameSettings.Map;

public delegate void OnPlayerMoved(IPlayer player, byte x, byte z);
public delegate void OnSelectWinner(IPlayer player);

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

    private byte[,] _maze, _defaultMaze;

    // Temporary solution for player trails //
    private byte[,] _trail;


    /// <summary>
    /// Events
    /// </summary>
    public static OnPlayerMoved OnPlayerMoved { get; private set; }
    public static OnSelectWinner OnSelectWinner { get; private set; }

    public static event Action<GameStatus> OnGameStatusChanged;
    public static event Action<byte[,]> OnMazeDataReceived;
    /// <summary>
    /// Events
    /// </summary>
    /// 

    private GameStatus _gameStatus;

    private MazeBuilder _mazeBuilder;
    private BoardUIManager _boardUIManager;
    private GameUIManager _gameUIControl;
    private EntitiesCollection _entitiesCollection;
    private IPlayer _localPlayer;


    #region Mono
    private void Awake()
    {
        DIContainer.Register(this);
    }
    private void Start()
    {
        _boardUIManager = DIContainer.Resolve<BoardUIManager>();
        _mazeBuilder = DIContainer.Resolve<MazeBuilder>();
        _entitiesCollection = DIContainer.Resolve<EntitiesCollection>();
        _gameUIControl = DIContainer.Resolve<GameUIManager>();
        _localPlayer = DIContainer.Resolve<IPlayer>();

        OnPlayerMoved += OnIPlayerMovedEvent;
        OnSelectWinner += OnWinnerSelectedEvent;

        ClockManager.OnClockReady += OnClockReady;

        BoardUIManager.OnUIEntityChange += OnUIEntityChangeEvent;

        GameUIManager.OnControlButtonPressed += OnUIControlBtnPressedEvent;
    }

    private void OnDestroy()
    {
        OnPlayerMoved -= OnIPlayerMovedEvent;

        ClockManager.OnClockReady -= OnClockReady;
        BoardUIManager.OnUIEntityChange -= OnUIEntityChangeEvent;
        GameUIManager.OnControlButtonPressed -= OnUIControlBtnPressedEvent;
    }
    #endregion
    #region Map
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
    public void OnUIEntityChangeEvent(byte entity, byte row, byte column)
    {
        _mazeBuilder.ModifyEntitiy(row, column, (EntityType)entity);
        SetMazeEntity(entity, row, column);
    }
    #endregion
    
    #region Game Events
    private void OnWinnerSelectedEvent(IPlayer player)
    {
        photonView.RPC(nameof(SelectWinnerRPC), PhotonTargets.All, player.GetNickname());
    }

    public void OnClockReady()
    {
        GameStatus = GameStatus.Playing;
    }
    #endregion
    #region Game Control
    public void OnUIControlBtnPressedEvent(GameControlType controlType)
    {
        switch (controlType)
        {
            case GameControlType.Start:
                StartGame();
                break;
            case GameControlType.Restart:
                ResetGame();
                break;
            case GameControlType.Stop:
                StopGame();
                break;
        }
    }

    private void StartGame() => _localPlayer.StartMovement();
    private void StopGame() => _localPlayer.StopMovement();
    private void ResetGame()
    {
        _localPlayer.ResetMovement();

        _maze = new byte[_defaultMaze.GetLength(0), _defaultMaze.GetLength(1)];
        _trail = new byte[_defaultMaze.GetLength(0), _defaultMaze.GetLength(1)];
        Buffer.BlockCopy(_defaultMaze, 0, _maze, 0, _defaultMaze.Length * sizeof(byte));

        OnMazeDataReceived?.Invoke(_maze);
        VisualizeMaze();
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
        _localPlayer.SetNickname("Player" + UnityEngine.Random.Range(100, 999));
        _gameUIControl.SetUIPlayerNickname(_localPlayer.GetNickname());
    }
    #endregion
    
    #region PunRPC
    [PunRPC]
    public void SelectWinnerRPC(string winnerNickname)
    {
        _localPlayer.StopMovement();
        
        _gameUIControl.DisplayUIWinner(winnerNickname);
    }
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
