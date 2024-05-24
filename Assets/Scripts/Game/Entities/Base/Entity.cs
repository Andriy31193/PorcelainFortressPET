using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : ScriptableObject
{
    public EntityType Type { get; protected set; }
    
    [SerializeField] private string _prefabPath = string.Empty;
    [SerializeField] protected List<EntityProperty> _customProperties;



    public string GetPrefabPath() => _prefabPath; 

    public abstract void Affect(IPlayer player);
}
