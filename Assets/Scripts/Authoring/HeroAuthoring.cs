using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HeroAuthoring : MonoBehaviour
{
    public float MaxHP = 100f;
    public float AttackDamage = 10f;
    public float AttackRange = 15f;
    public float FireRate = 2f;     // shots per second
    public int Lane = 0;

    public class Baker : Baker<HeroAuthoring>
    {
        public override void Bake(HeroAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new HeroTag());

            AddComponent(entity, new Health
            {
                CurrentHealth = authoring.MaxHP,
                Max = authoring.MaxHP
            });

            AddComponent(entity, new HeroAttack
            {
                Damage = authoring.AttackDamage,
                Range = authoring.AttackRange,
                FireRate = authoring.FireRate,
                CooldownTimer = 0f
            });

            AddComponent(entity, new LaneIndex
            {
                Value = authoring.Lane
            });

            AddComponent(entity, new TargetEntity
            {
                Value = Entity.Null
            });
        }
    }
}