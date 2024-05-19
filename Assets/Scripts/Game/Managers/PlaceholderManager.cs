using UnityEngine;

public sealed class PlaceholderManager : MonoBehaviour
{
    private GameObject _placeholderCube;

    private void Start()
    {
        _placeholderCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _placeholderCube.GetComponent<Collider>().enabled = false; 
        _placeholderCube.GetComponent<Renderer>().material.color = Color.red;
        _placeholderCube.SetActive(false);
    }

    public void UpdatePlaceholderPosition(Vector3 position)
    {
        _placeholderCube.transform.position = position;
        _placeholderCube.SetActive(true);
    }

    public void HidePlaceholder()
    {
        _placeholderCube.SetActive(false);
    }
}
