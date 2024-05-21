using UnityEngine;

public interface IPlayer
{
    void TakeDamage(int damage);
    void ChangeDirection(Direction direction);
}
