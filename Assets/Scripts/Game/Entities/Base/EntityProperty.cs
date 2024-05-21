using UnityEngine;

[System.Serializable]
public class EntityProperty
{
    [SerializeField] private string _name = string.Empty;
    [SerializeField] private string _value = string.Empty;

    public string GetName() => _name;
    public string GetValue() => _value;
}
