using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class BoardUIBuilder : MonoBehaviour
{
    public static BoardUIBuilder Instance { get; private set; }

    [SerializeField] private GridLayoutGroup _boardParent;
    [SerializeField] private HorizontalLayoutGroup _toolsParent;

    private GameObject _boardCellPrefab = null;
    private GameObject _toolCellPrefab = null;

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
    private void OnRemoveCell(byte row, byte column)
    {
        MazeGenerator.Instance.ModifyEntitiy(row, column, EntityType.Void);
    }
    public void Build(byte[,] board)
    {
        _boardParent.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _boardParent.constraintCount = board.GetLength(0) - 1;

        for (byte row = 0; row < board.GetLength(0); row++)
        {
            for (byte column = 0; column < board.GetLength(1); column++)
            {
                byte nrow = row;
                byte ncolumn = column;
                GameObject cell = Instantiate(_boardCellPrefab, _boardParent.transform);
                cell.GetComponent<Button>().onClick.AddListener(() => OnRemoveCell(nrow, ncolumn));
                cell.GetComponent<Image>().color = board[row, column] == 1? Color.red: Color.white;
                cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = row+","+column+"("+board[row, column].ToString()+")";
            }
        }
    }
}
