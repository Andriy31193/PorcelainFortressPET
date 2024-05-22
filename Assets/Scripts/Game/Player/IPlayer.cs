using UnityEngine;

public interface IPlayer
{
    void StartMovement();
    void ResetMovement();
    void StopMovement();
    void Finish();

    void TakeDamage(int damage);
    void ChangeDirection(Direction direction);
}
