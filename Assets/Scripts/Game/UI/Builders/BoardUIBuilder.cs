using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class BoardUIBuilder : MonoBehaviour
{
    private const string PREFAB_PATH = "UI/BoardCell";

    public static Dictionary<Vector2Int, KeyValuePair<EntityType, GameObject>> Build(byte[,] board, GridLayoutGroup parent)
    {
        var boardCellPrefab = Resources.Load<GameObject>(PREFAB_PATH);
        var uiMaze = new Dictionary<Vector2Int, KeyValuePair<EntityType, GameObject>>();

        for (byte row = 0; row < board.GetLength(0); row++)
        {
            for (byte column = 0; column < board.GetLength(1); column++)
            {
                Vector2Int currentPosition = new (row, column);
                GameObject cell = Object.Instantiate(boardCellPrefab, parent.transform);
                uiMaze.Add(currentPosition, new KeyValuePair<EntityType, GameObject>((EntityType)board[row, column], cell));
            }
        }

        return uiMaze;
    }
}
