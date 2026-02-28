using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(HeroAttackSystem))]
public partial struct DamageApplySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (health, damageBuffer, entity) in
            SystemAPI.Query<RefRW<Health>, DynamicBuffer<DamageBuffer>>()
                .WithNone<DestroyTag>()
                .WithEntityAccess())
        {
            if (damageBuffer.Length == 0) continue;

            float totalDamage = 0f;
            for (int i = 0; i < damageBuffer.Length; i++)
            {
                totalDamage += damageBuffer[i].Amount;
            }
            damageBuffer.Clear();

            health.ValueRW.CurrentHealth -= totalDamage;

            // If dead and it's an enemy, mark for destroy
            if (health.ValueRO.CurrentHealth <= 0f)
            {
                if (state.EntityManager.HasComponent<EnemyTag>(entity))
                {
                    ecb.AddComponent<DestroyTag>(entity);
                }
                // Castle death handled by GameStateSystem
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}