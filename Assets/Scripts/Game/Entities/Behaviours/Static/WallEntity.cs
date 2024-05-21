using UnityEngine;

[CreateAssetMenu(fileName = "Wall", menuName = "Entities/Wall")]
public sealed class WallEntity : Entity
{

    public WallEntity() => Type = EntityType.Wall;

    public override void Affect(IPlayer player) {}
}
