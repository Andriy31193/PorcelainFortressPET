using UnityEngine;

[CreateAssetMenu(fileName = "RotateLeft", menuName = "Entities/RotateLeft")]
public sealed class RotateLeftEntity : Entity
{
    private Direction _direction = Direction.None;

    public RotateLeftEntity() => Type = EntityType.RotateLeft;
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
