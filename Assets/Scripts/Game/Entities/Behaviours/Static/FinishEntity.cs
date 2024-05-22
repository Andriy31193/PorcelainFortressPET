using UnityEngine;

[CreateAssetMenu(fileName = "Finish", menuName = "Entities/Finish")]
public sealed class FinishEntity : Entity
{

    public FinishEntity() => Type = EntityType.Finish;

    public override void Affect(IPlayer player) 
    {
        player.Finish();
    }
}
