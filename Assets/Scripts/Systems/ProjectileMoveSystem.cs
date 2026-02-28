using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(DamageApplySystem))]
public partial struct ProjectileMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float hitDistance = 0.3f;

        foreach (var (projectile, transform, entity) in
            SystemAPI.Query<RefRW<ProjectileData>, RefRW<LocalTransform>>()
                .WithAll<ProjectileTag>()
                .WithNone<DestroyTag>()
                .WithEntityAccess())
        {
            // Lifetime check
            projectile.ValueRW.LifeTime -= dt;
            if (projectile.ValueRO.LifeTime <= 0f)
            {
                ecb.AddComponent<DestroyTag>(entity);
                continue;
            }

            // Check target still valid
            if (projectile.ValueRO.Target == Entity.Null ||
                !state.EntityManager.Exists(projectile.ValueRO.Target) ||
                state.EntityManager.HasComponent<DestroyTag>(projectile.ValueRO.Target))
            {
                ecb.AddComponent<DestroyTag>(entity);
                continue;
            }

            // Move toward target
            var targetTransform = state.EntityManager.GetComponentData<LocalTransform>(projectile.ValueRO.Target);
            float3 dir = math.normalize(targetTransform.Position - transform.ValueRO.Position);
            transform.ValueRW.Position += dir * projectile.ValueRO.Speed * dt;

            // Hit check
            float dist = math.distance(transform.ValueRO.Position, targetTransform.Position);
            if (dist <= hitDistance)
            {
                // Apply damage
                if (state.EntityManager.HasBuffer<DamageBuffer>(projectile.ValueRO.Target))
                {
                    var buffer = state.EntityManager.GetBuffer<DamageBuffer>(projectile.ValueRO.Target);
                    buffer.Add(new DamageBuffer { Amount = projectile.ValueRO.Damage });
                }

                ecb.AddComponent<DestroyTag>(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}