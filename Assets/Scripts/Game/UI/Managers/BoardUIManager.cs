using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public delegate void OnUIEntityChange(byte entity, byte row, byte column);
public sealed class BoardUIManager : MonoBehaviour
{
    public static OnUIEntityChange OnUIEntityChange { get; set; }

    [SerializeField] private GridLayoutGroup _boardParent;

    private Dictionary<Vector2Int, KeyValuePair<EntityType, GameObject>> _uiMaze = new();
    private GameManager _gameManager;
    private ToolsUIManager _toolsUI;

    private void Awake()
    {
        DIContainer.Register(this);
        GameManager.OnMazeDataReceived += CreateBoard;
    }

    private void Start()
    {
        _toolsUI = DIContainer.Resolve<ToolsUIManager>();


    }
    private void OnDestroy() {
        GameManager.OnMazeDataReceived -= CreateBoard;
    }

    public void OnAffectCell(byte row, byte column)
    {
        OnUIEntityChange?.Invoke((byte)_toolsUI.CurrentTool, row, column);
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
            case EntityType.Finish:
                color = Color.green;
                break;
            case EntityType.Hole:
                color = Color.black;
                break;
        }

        cell.GetComponent<Button>().onClick.AddListener(() => OnAffectCell(row, column));
        cell.GetComponent<Image>().color = color;
        var textUI = cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textUI.color = color == Color.black ? Color.white : Color.black;
        textUI.text = row + "," + column + "(" + entityType.ToString() + ")";
    }
    private void CreateBoard(byte[,] board)
    {
        _boardParent.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _boardParent.constraintCount = board.GetLength(0);

        if (_uiMaze.Count == 0)
            _uiMaze = BoardUIBuilder.Build(board, _boardParent);

        RefreshBoard(board, true);
    }
}
