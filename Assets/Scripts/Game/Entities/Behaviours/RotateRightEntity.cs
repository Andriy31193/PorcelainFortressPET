using UnityEngine;

[CreateAssetMenu(fileName = "RotateRight", menuName = "Entities/RotateRight")]
public sealed class RotateRightEntity : Entity
{
    private Direction _direction = Direction.None;

    public RotateRightEntity() => Type = EntityType.RotateRight;
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
