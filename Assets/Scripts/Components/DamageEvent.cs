using Unity.Entities;

public struct DamageEvent : IComponentData
{
    public Entity Target;
    