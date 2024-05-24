using UnityEngine;

public interface IPlayer
{

    string GetNickname();
    void SetNickname(string value);

    void StartMovement();
    void ResetMovement();
    void StopMovement();
    void Finish();

    void TakeDamage(int damage);
    void ChangeDirection(DirectionType direction);
}
