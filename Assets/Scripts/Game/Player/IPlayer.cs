using UnityEngine;

public interface IPlayer
{
    void StartMovement();
    void StopMovement();

    void TakeDamage(int damage);
    void ChangeDirection(Direction direction);
}
