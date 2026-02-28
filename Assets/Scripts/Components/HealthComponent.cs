using Unity.Entities;

public struct HealthComponent : IComponentData
{
    public float MaxHealth;
    public float CurrentHealth;
}