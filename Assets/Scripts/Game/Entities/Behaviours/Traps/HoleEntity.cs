using UnityEngine;

[CreateAssetMenu(fileName = "Hole", menuName = "Entities/Hole")]
public sealed class HoleEntity : Entity
{
    public HoleEntity() => Type = EntityType.Hole;

    public override void Affect(IPlayer player)
    {
        Debug.Log("Game over");
    }
}
