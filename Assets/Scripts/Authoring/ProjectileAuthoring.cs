using Unity.Entities;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{
    public float Speed = 20f;
    public float Damage = 10f;
    public float LifeTime = 3f;

    public class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ProjectileTag());

            AddComponent(entity, new ProjectileData
            {
                Speed = authoring.Speed,
                Damage = authoring.Damage,
                Target = Entity.Null,
                LifeTime = authoring.LifeTime
            });
        }
    }
}