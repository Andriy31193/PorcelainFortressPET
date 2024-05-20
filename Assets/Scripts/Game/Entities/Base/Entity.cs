using UnityEngine;

public abstract class Entity : ScriptableObject
{
    public EntityType Type { get; protected set; }
    protected Transform Player => PlayerController.Instance.transform;


    public abstract void Affect();
}
