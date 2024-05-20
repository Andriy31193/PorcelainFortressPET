using System.Collections.Generic;
using UnityEngine;

public sealed class EntitiesCollection : MonoBehaviour
{
    public static EntitiesCollection Instance { get; private set; }

    [SerializeField]
    private List<Entity> _entities;

    private void Awake() {
        if(Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }

    public Entity GetEntitity(EntityType entityType)
    {
        Entity entity = _entities.Find(x => x.Type == entityType);

        if(entity == null)
        { 
            Debug.LogError("Couldn't find entity with type: " + entityType.ToString());
            return null;
        }

        return entity; 
    }
}
