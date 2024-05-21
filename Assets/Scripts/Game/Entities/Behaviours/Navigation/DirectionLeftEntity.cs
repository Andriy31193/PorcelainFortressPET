using UnityEngine;

[CreateAssetMenu(fileName = "DirectionLeft", menuName = "Entities/DirectionLeft")]
public sealed class DirectionLeftEntity : Entity
{

    public DirectionLeftEntity() => Type = EntityType.DirectionLeft;

    public override void Affect(IPlayer player)
    {
        player.ChangeDirection(Direction.Left);
    }
}
