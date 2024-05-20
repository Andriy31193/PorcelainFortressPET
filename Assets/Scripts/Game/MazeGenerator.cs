using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Map = GameSettings.Map;

public class MazeGenerator : Photon.PunBehaviour
{
    public static MazeGenerator Instance { get; private set; }
    private readonly Stack<Vector2Int> _stack = new();
    private readonly List<Vector2Int> _directions = new()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };
    private byte[,] _maze;
    private Dictionary<KeyValuePair<byte, byte>, GameObject> _3dMaze;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
    public void Generate()
    {
        GenerateMaze();
        GenerateChallenges();

        byte[] mazeData = new byte[Map.Rows * Map.Columns];
        Buffer.BlockCopy(_maze, 0, mazeData, 0, _maze.Length);

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("OnReceiveMazeData", PhotonTargets.AllBuffered, mazeData);
    }
    private void GenerateMaze()
    {
        _maze = new byte[Map.Rows, Map.Columns];


        Vector2Int current = new(0, 0);
        _stack.Push(current);
        _maze[current.x, current.y] = (byte)EntityType.Wall;

        while (_stack.Count > 0)
        {
            current = _stack.Pop();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                _stack.Push(current);
                Vector2Int chosen = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
                RemoveWall(current, chosen);
                _maze[chosen.x, chosen.y] = (byte)EntityType.Wall;
                _stack.Push(chosen);
            }
        }

        for (int row = 0; row < Map.Rows; row++)
        {
            for (int col = 0; col < Map.Columns; col++)
            {
                if (row == 0 || row == Map.Rows - 1 || col == 0 || col == Map.Columns - 1)
                {
                    _maze[row, col] = (byte)EntityType.Wall;
                }
            }
        }
        _maze[0, 1] = (byte)EntityType.Void;
    }

    private void GenerateChallenges()
    {
        for (byte i = 0; i < 10; i++)
        {
            Entity randomChallenge = EntitiesCollection.Instance.GetRandomEntitity(ignore: EntityType.Wall);
            _maze[UnityEngine.Random.Range(1, (int)Map.Rows -1), (int)UnityEngine.Random.Range(1, Map.Columns -1)] = (byte)randomChallenge.Type;

        }
    }
    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new();

        foreach (var direction in _directions)
        {
            Vector2Int neighbor = cell + direction * 2;

            if (IsInBounds(neighbor) && _maze[neighbor.x, neighbor.y] == 0)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private void RemoveWall(Vector2Int current, Vector2Int chosen)
    {
        Vector2Int wall = (current + chosen) / 2;
        _maze[wall.x, wall.y] = 1;
    }

    private bool IsInBounds(Vector2Int position) => position.x >= 0 && position.x < Map.Rows && position.y >= 0 && position.y < Map.Columns;


    private void BuildMaze(byte[,] data)
    {
        _3dMaze = new Dictionary<KeyValuePair<byte, byte>, GameObject>();
        for (byte row = 0; row < data.GetLength(0); row++)
        {
            for (byte column = 0; column < data.GetLength(1); column++)
            {

                var entity = HandleCreation(data[row, column], new Vector3(-4.5f + column, 0.5f, 4.5f - row));

                _3dMaze.Add(new KeyValuePair<byte, byte>(row, column), entity);
            }
        }
    }
    public GameObject HandleCreation(byte entityType, Vector3 position)
    {
        //Entity entity = EntitiesCollection.Instance.GetEntitity((EntityType)entityType);
        string path = string.Empty;

        switch ((EntityType)entityType)
        {
            case EntityType.Hole:
                path = "Entities/Traps/Hole";
                break;
            case EntityType.Wall:
                path = "Entities/Neutral/Wall";
                break;
        }

        if (!string.IsNullOrEmpty(path))
        {
            GameObject entity = Resources.Load<GameObject>(path);

            if (entity != null)
            {
                return Instantiate(entity, position, entity.transform.rotation);
            }
        }
        return null;
    }
    public void ModifyEntitiy(byte row, byte column, EntityType newEntity)
    {
        var d3Object = _3dMaze.FirstOrDefault(x => x.Key.Key == row && x.Key.Value == column);
        if (d3Object.Value != null)
        {
            _maze[row, column] = (byte)newEntity;
            Destroy(_3dMaze[d3Object.Key]);
            _3dMaze[d3Object.Key] = null;
        }
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
        Buffer.BlockCopy(data, 0, _maze, 0, data.Length);
        BuildMaze(_maze);
        BoardUIBuilder.Instance.Build(_maze);

    }
}
