using UnityEngine;

[CreateAssetMenu(fileName = "RotatorEntity", menuName = "Entities/RotatorEntity")]
public sealed class RotatorEntity : Entity
{
    private Direction _direction = Direction.None;

    public RotatorEntity() => Type = EntityType.Rotator;
    public void SetDirection(Direction direction) => _direction = direction;

    public override void Affect()
    {
        switch (_direction)
        {
            case Direction.Left:
                Player.Rotate(90, 0, 0);
            break;
            case Direction.Right:
                Player.Rotate(-90, 0, 0);
            break;
        }
    }
}
