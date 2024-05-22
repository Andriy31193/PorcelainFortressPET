using UnityEngine;
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _chessboardPlane;
    private GameObject _placeholderCube;
    private GameManager _gm;
    private EntityType _currentTool = EntityType.Void;
    private GameObject _defaultToolObject;

    private void Awake()
    {
        ToolsUIManager.OnCurrentToolSwitched += RefreshPlaceholderObject;
    }
    private void Start()
    {
        _gm = DIContainer.Resolve<GameManager>();

        SetDefaultToolObject(GameObject.CreatePrimitive(PrimitiveType.Cube));
        RefreshPlaceholderObject(EntityType.Void);
    }
    private void SetDefaultToolObject(GameObject obj)
    {
        _defaultToolObject = obj;
        _defaultToolObject.GetComponent<Collider>().enabled = false;
        _defaultToolObject.GetComponent<Renderer>().material.color = Color.red;
    }
    private void RefreshPlaceholderObject(EntityType newEntity)
    {
        _currentTool = newEntity;

        _placeholderCube = EntityUtils.HandleCreation(newEntity, Vector3.zero);

        if(_placeholderCube == null)
            _placeholderCube = _defaultToolObject;
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 localHitPoint = _chessboardPlane.InverseTransformPoint(hit.point);

            int cellX = Mathf.FloorToInt(localHitPoint.x);
            int cellZ = Mathf.FloorToInt(localHitPoint.z);

            Vector3 placeholderPosition = _chessboardPlane.TransformPoint(new Vector3(cellX + 0.5f, 0.5f, cellZ + 0.5f));

            _placeholderCube.transform.position = placeholderPosition;
            _placeholderCube.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                byte mazeX = (byte)Mathf.RoundToInt(4.5f - placeholderPosition.z);
                byte mazeZ = (byte)Mathf.RoundToInt(placeholderPosition.x + 4.5f);
                _gm.HandleCreation(mazeX, mazeZ, _currentTool);
                _placeholderCube.SetActive(false);
            }
        }
        else
        {
            _placeholderCube.SetActive(false);
        }
    }
}
