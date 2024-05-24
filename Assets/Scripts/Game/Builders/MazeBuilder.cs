using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Map = GameSettings.Map;

public class MazeBuilder : Photon.PunBehaviour
{
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
    private List<GameObject> _customCreations = new();

    private void Awake()
    {
        DIContainer.Register(this);
    }
    private void Start()
    {
        GameManager.OnMazeDataReceived += OnMazeDataReceived;
    }
    private void OnMazeDataReceived(byte[,] data)
    {
        Build3DMaze(data);
    }
    public void Generate(Action<byte[]> onMazeGenerated)
    {

        GenerateMaze();


        byte[] mazeData = new byte[Map.Rows * Map.Columns];
        Buffer.BlockCopy(_maze, 0, mazeData, 0, _maze.Length);

        onMazeGenerated?.Invoke(mazeData);
    }
    private void GenerateMaze()
    {
        _maze = new byte[Map.Rows, Map.Columns];
        GenerateChallenges();

        #region Procdural Generation
        // Vector2Int current = new(0, 0);
        // _stack.Push(current);
        // _maze[current.x, current.y] = (byte)EntityType.Wall;

        // while (_stack.Count > 0)
        // {
        //     current = _stack.Pop();
        //     List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

        //     if (neighbors.Count > 0)
        //     {
        //         _stack.Push(current);
        //         Vector2Int chosen = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
        //         RemoveWall(current, chosen);
        //         _maze[chosen.x, chosen.y] = (byte)EntityType.Wall;
        //         _stack.Push(chosen);
        //     }
        // }

        


        // for (int row = 0; row < Map.Rows; row++)
        // {
        //     for (int col = 0; col < Map.Columns; col++)
        //     {
        //         if (row == 0 || row == Map.Rows - 1 || col == 0 || col == Map.Columns - 1)
        //         {
        //             _maze[row, col] = (byte)EntityType.Wall;
        //         }
        //     }
        // }
        #endregion

    }

    private void GenerateChallenges()
    {

        for (byte i = 0; i < 10; i++)
        {
            Entity randomChallenge = DIContainer.Resolve<EntitiesCollection>().GetRandomEntitity(EntityType.Finish, EntityType.DirectionLeft, EntityType.DirectionRight);
            _maze[UnityEngine.Random.Range(1, (int)Map.Rows - 1), (int)UnityEngine.Random.Range(1, Map.Columns - 1)] = (byte)randomChallenge.Type;

        }
        _maze[UnityEngine.Random.Range(1, (int)Map.Rows - 1), (int)UnityEngine.Random.Range(1, Map.Columns - 1)] = (byte)EntityType.Finish;
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
    private void ClearCustomCreations()
    {
        _customCreations.ForEach(x => Destroy(x));
        _customCreations.Clear();
    }
    public void Build3DMaze(byte[,] data)
    {
        ClearCustomCreations();

        _3dMaze = new Dictionary<KeyValuePair<byte, byte>, GameObject>();
        for (byte row = 0; row < data.GetLength(0); row++)
        {
            for (byte column = 0; column < data.GetLength(1); column++)
            {
                var entity = HandleCreation((EntityType)data[row, column], MazeUtils.MatrixTo3DPosition(row, column));

                _3dMaze.Add(new KeyValuePair<byte, byte>(row, column), entity);
            }
        }
    }
    public GameObject HandleCreation(EntityType entityType, Vector3 position)
    {
        if (entityType == EntityType.Void)
            return null;


        Entity entity = DIContainer.Resolve<EntitiesCollection>().GetEntitity(entityType);

        if (entity != null && !string.IsNullOrEmpty(entity.GetPrefabPath()))
        {
            GameObject creation = Resources.Load<GameObject>(entity.GetPrefabPath());

            if (creation != null)
            {
                return Instantiate(creation, position, creation.transform.rotation);
            }
            else Debug.LogError($"Resource with path: {entity.GetPrefabPath()} was not found.");
        }
        return null;
    }
    public void ModifyEntitiy(byte row, byte column, EntityType newEntity)
    {
        if(_3dMaze == null)
            return;

        var key = new KeyValuePair<byte, byte>(row, column);
        if (_3dMaze.TryGetValue(key, out GameObject d3Object))
        {
            GameObject go = HandleCreation(newEntity, MazeUtils.MatrixTo3DPosition(row, column));

            if (go != null)
            {
                _customCreations.Add(go);

                if(_3dMaze[key] != null)
                    Destroy(_3dMaze[key]);
            }

            _3dMaze[key] = go;

            if (newEntity == EntityType.Void && d3Object != null)
            {
                Destroy(d3Object);
                _3dMaze[key] = null;
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnMazeDataReceived -= OnMazeDataReceived;
    }
}
