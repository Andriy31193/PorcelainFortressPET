using System;
using System.Collections.Generic;
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
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
    public void Generate()
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

        byte[] mazeData = new byte[Map.Rows * Map.Columns];
        Buffer.BlockCopy(_maze, 0, mazeData, 0, _maze.Length);

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("OnReceiveMazeData", PhotonTargets.AllBuffered, mazeData);
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

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

        for (int row = 0; row < data.GetLength(0); row++)
        {
            for (int column = 0; column < data.GetLength(1); column++)
            {
                if (data[row, column] == 1)
                    HandleCreation(data[row, column], new Vector3(-4.5f + row, 0.5f, 4.5f - column));
            }
        }
    }
    public void HandleCreation(byte entityType, Vector3 position)
    {
        Entity entity = EntitiesCollection.Instance.GetEntitity((EntityType)entityType);
        entity.Affect();
        switch  ((EntityType)entityType)
        {
            case EntityType.Wall:
                GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newCube.transform.position = position;
            break;
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
    }
}
