using Unity.Entities;
using UnityEngine;

public class CastleAuthoring : MonoBehaviour
{
    public float MaxHP = 1000f;
    public float CastleLineY = -4f;

    public class Baker : Baker<CastleAuthoring>
    {
        public override void Bake(CastleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CastleTag());

            AddComponent(entity, new Health
            {
                CurrentHealth = authoring.MaxHP,
                Max = authoring.MaxHP
            });

            AddComponent(entity, new CastleLine
            {
                YPosition = authoring.CastleLineY
            });

            AddBuffer<DamageBuffer>(entity);
        }
    }
}