using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float MaxHP = 30f;
    public float Speed = 3f;
    public float DamageToCastle = 10f;
    public int ScoreValue = 10;
    public int Lane = 0;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new EnemyTag());

            AddComponent(entity, new Health
            {
                CurrentHealth = authoring.MaxHP,
                Max = authoring.MaxHP
            });

            AddComponent(entity, new MoveSpeedComponent
            {
                Value = authoring.Speed
            });

            AddComponent(entity, new MoveDirection
            {
                Value = new Unity.Mathematics.float3(0f, -1f, 0f)
            });

            AddComponent(entity, new EnemyStats
            {
                Damage = authoring.DamageToCastle,
                ScoreValue = authoring.ScoreValue
            });

            AddComponent(entity, new LaneIndex
            {
                Value = authoring.Lane
            });

            AddBuffer<DamageBuffer>(entity);
        }
    }
}