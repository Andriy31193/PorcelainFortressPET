using UnityEngine;

[CreateAssetMenu(fileName = "WallEntity", menuName = "Entities/WallEntity")]
public sealed class WallEntity : Entity
{

    public WallEntity() => Type = EntityType.Wall;

    public override void Affect()
    {
        
        Debug.Log("Wall affected");
    }
}
