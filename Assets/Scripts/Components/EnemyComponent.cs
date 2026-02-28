using Unity.Entities;

public struct EnemyComponent : IComponentData
{
    public int EnemyTypeId;
    public int GoldReward;
    public int DamageToBase;
}