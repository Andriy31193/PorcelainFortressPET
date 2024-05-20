using System;
using System.Collections.Generic;
using System.Linq;
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
        Entity entity = _entities.Find(x=>x.Type == entityType);

        if(entity == null)
        { 
            Debug.LogError("Couldn't find entity with type: " + entityType.ToString());
            return null;
        }

        return entity; 
    }
    public Entity GetRandomEntitity(params EntityType[] ignore)
    {
        var entities = _entities.Where(x => !ignore.Contains(x.Type)).ToArray();
        Entity entity = entities[UnityEngine.Random.Range(0, entities.Length)];

        if(entity == null)
        { 
            Debug.LogError("Couldn't find entity with type: " + entity.Type.ToString());
            return null;
        }

        return entity; 
    }
}
