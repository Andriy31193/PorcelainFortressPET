using UnityEngine;
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _chessboardPlane;
    private GameObject _placeholderCube;

    private void Start()
    {
        _placeholderCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _placeholderCube.GetComponent<Collider>().enabled = false;
        _placeholderCube.GetComponent<Renderer>().material.color = Color.red; 
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == _chessboardPlane)
        {
            Vector3 localHitPoint = _chessboardPlane.InverseTransformPoint(hit.point);

            int cellX = Mathf.FloorToInt(localHitPoint.x);
            int cellZ = Mathf.FloorToInt(localHitPoint.z);

            Vector3 placeholderPosition = _chessboardPlane.TransformPoint(new Vector3(cellX + 0.5f, 0, cellZ + 0.5f));

            _placeholderCube.transform.position = placeholderPosition;
            _placeholderCube.SetActive(true);

            if (Input.GetMouseButtonDown(0))
                GameManager.Instance.HandleCreation(placeholderPosition);
        }
        else
        {
            _placeholderCube.SetActive(false);
        }
    }
}
