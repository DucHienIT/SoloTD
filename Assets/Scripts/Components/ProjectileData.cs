using Unity.Entities;

public struct ProjectileData : IComponentData
{
    public float Damage;
    public float Speed;
    public Entity Target;
    public float LifeTime;
}