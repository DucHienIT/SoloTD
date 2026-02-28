using Unity.Entities;

public struct HeroAttack : IComponentData
{
    public float Damage;
    public float Range;
    public float FireRate;
    public float CooldownTimer;
}