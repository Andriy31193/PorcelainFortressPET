using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class EntitiesCollection : MonoBehaviour
{
    private const string SPRITES_PATH = "";
    [SerializeField] private List<Entity> _entities;


    private void Awake()
    {
        DIContainer.Register(this);
    }
    public static Sprite GetEntityUISprite(EntityType entityType)
    {
        var sprite = Resources.Load<Sprite>($"{SPRITES_PATH}{entityType.ToString("G")}");
        if (sprite == null)
        {
            Debug.LogError("Couldn't find sprite for entity type:" + entityType.ToString("G"));
            return null;
        }

        return sprite;
    }
    public Entity[] GetAll(params EntityType[] ignore)
    {
        return _entities.Where(x => !ignore.Contains(x.Type)).ToArray();
    }
    public Entity GetEntitity(EntityType entityType)
    {
        if (entityType == EntityType.Void)
            return null;

        Entity entity = _entities.Find(x => x.Type == entityType);

        if (entity == null)
        {
            Debug.LogError("Couldn't find entity with type: " + entityType.ToString());
            return null;
        }

        return entity;
    }
    public Entity GetRandomEntitity(params EntityType[] ignore)
    {
        var entities = GetAll(ignore);
        Entity entity = entities[UnityEngine.Random.Range(0, entities.Length)];

        if (entity == null)
        {
            Debug.LogError("Couldn't find entity with type: " + entity.Type.ToString());
            return null;
        }

        return entity;
    }
}
