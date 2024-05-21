using UnityEngine;

[CreateAssetMenu(fileName = "DirectionRight", menuName = "Entities/DirectionRight")]
public sealed class DirectionRightEntity : Entity
{
    public DirectionRightEntity() => Type = EntityType.DirectionRight;

    public override void Affect(IPlayer player)
    {
        player.ChangeDirection(Direction.Right);
    }
}
