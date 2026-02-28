using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemyMoveSystem))]
public partial struct CastleReachCheckSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CastleTag>();
        state.RequireForUpdate<EnemyTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Find castle line Y
        float castleLineY = -4f;
        Entity castleEntity = Entity.Null;

        foreach (var (castleLine, health, entity) in
            SystemAPI.Query<RefRO<CastleLine>, RefRO<Health>>()
                .WithAll<CastleTag>()
                .WithEntityAccess())
        {
            castleLineY = castleLine.ValueRO.YPosition;
            castleEntity = entity;
            break; // single castle
        }

        if (castleEntity == Entity.Null) return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transform, enemyStats, entity) in
            SystemAPI.Query<RefRO<LocalTransform>, RefRO<EnemyStats>>()
                .WithAll<EnemyTag>()
                .WithNone<DestroyTag>()
                .WithEntityAccess())
        {
            if (transform.ValueRO.Position.y <= castleLineY)
            {
                // Apply damage to castle via DamageBuffer
                var castleBuffer = SystemAPI.GetBuffer<DamageBuffer>(castleEntity);
                castleBuffer.Add(new DamageBuffer { Amount = enemyStats.ValueRO.Damage });

                // Mark enemy for destruction
                ecb.AddComponent<DestroyTag>(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}