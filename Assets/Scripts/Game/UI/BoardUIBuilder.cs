using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnUIEntityChange(byte entity, byte row, byte column);
public sealed class BoardUIBuilder : MonoBehaviour
{
    public static BoardUIBuilder Instance { get; private set; }

    public static OnUIEntityChange OnUIEntityChange { get; set; }

    [SerializeField] private GridLayoutGroup _boardParent;
    [SerializeField] private HorizontalLayoutGroup _toolsParent;

    private GameObject _boardCellPrefab = null;
    private GameObject _toolCellPrefab = null;
    private Dictionary<Vector2Int, KeyValuePair<EntityType, GameObject>> _uiMaze = new();
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
    private void Start()
    {
        _boardCellPrefab = Resources.Load<GameObject>("UI/BoardCell");
        _toolCellPrefab = Resources.Load<GameObject>("UI/ChallengeTool");
    }
    public void OnRemoveCell(byte row, byte column)
    {
        OnUIEntityChange?.Invoke(0, row, column);
    }
    public void RefreshBoard(byte[,] board, bool ignoreNewTypes = false)
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int column = 0; column < board.GetLength(1); column++)
            {
                var element = _uiMaze.FirstOrDefault(x => x.Key.x == row && x.Key.y == column);
                var newType = (EntityType)board[row, column];

                if (element.Value.Value != null && (newType != element.Value.Key || ignoreNewTypes))
                {
                    _uiMaze[element.Key] = new(newType, element.Value.Value);
                    RefreshCell(element.Key, newType, element.Value.Value);
                }
            }
        }
    }
    private void RefreshCell(Vector2Int position, EntityType entityType, GameObject cell)
    {
        byte row = (byte)position.x;
        byte column = (byte)position.y;

        Color color = Color.white;

        switch (entityType)
        {
            case EntityType.Wall:
                color = Color.red;
                break;
            case EntityType.Player:
                color = Color.blue;
                break;
        }

        cell.GetComponent<Button>().onClick.AddListener(() => OnRemoveCell(row, column));
        cell.GetComponent<Image>().color = color;
        cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = row + "," + column + "(" + entityType.ToString() + ")";
    }
    public void Build(byte[,] board)
    {
        _boardParent.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _boardParent.constraintCount = board.GetLength(0) - 1;


        for (byte row = 0; row < board.GetLength(0); row++)
        {
            for (byte column = 0; column < board.GetLength(1); column++)
            {
                EntityType currentEntityType = (EntityType)board[row, column];
                Vector2Int currentPosition = new(row, column);

                GameObject cell = Instantiate(_boardCellPrefab, _boardParent.transform);
                KeyValuePair<EntityType, GameObject> key = new(currentEntityType, cell);
                _uiMaze.Add(currentPosition, key);
            }
        }

        RefreshBoard(board, ignoreNewTypes: true);
    }

}
