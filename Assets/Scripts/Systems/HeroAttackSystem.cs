using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(HeroTargetingSystem))]
public partial struct HeroAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HeroTag>();
        state.RequireForUpdate<EnemyPrefabReference>(); // reuse as signal manager exists
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // Get projectile prefab - we need a prefab for projectiles
        // For MVP, we apply damage directly instead of spawning projectiles
        // This avoids needing a projectile prefab initially

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (attack, target, heroTransform, entity) in
            SystemAPI.Query<RefRW<HeroAttack>, RefRO<TargetEntity>, RefRO<LocalTransform>>()
                .WithAll<HeroTag>()
                .WithEntityAccess())
        {
            // Cooldown tick
            attack.ValueRW.CooldownTimer -= dt;

            if (attack.ValueRO.CooldownTimer > 0f) continue;
            if (target.ValueRO.Value == Entity.Null) continue;

            // Validate target still exists and has DamageBuffer
            if (!state.EntityManager.Exists(target.ValueRO.Value)) continue;
            if (!state.EntityManager.HasComponent<DamageBuffer>(target.ValueRO.Value)) continue;
            if (state.EntityManager.HasComponent<DestroyTag>(target.ValueRO.Value)) continue;

            // Check range
            var targetTransform = state.EntityManager.GetComponentData<LocalTransform>(target.ValueRO.Value);
            float dist = math.distance(heroTransform.ValueRO.Position, targetTransform.Position);
            if (dist > attack.ValueRO.Range) continue;

            // Apply damage directly via DamageBuffer (MVP - no projectile entity)
            var damageBuffer = state.EntityManager.GetBuffer<DamageBuffer>(target.ValueRO.Value);
            damageBuffer.Add(new DamageBuffer { Amount = attack.ValueRO.Damage });

            // Reset cooldown
            attack.ValueRW.CooldownTimer = 1f / attack.ValueRO.FireRate;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}