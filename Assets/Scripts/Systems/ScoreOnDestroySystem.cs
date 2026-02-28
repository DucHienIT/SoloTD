using Unity.Entities;
using Unity.Collections;

/// <summary>
/// Awards score just before destruction. Runs once per destroy-tagged enemy.
/// Uses a ScoredTag to prevent double counting.
/// </summary>
public struct ScoredTag : IComponentData { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(GameStateSystem))]
[UpdateBefore(typeof(DestroySystem))]
public partial struct ScoreOnDestroySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameState>();
    }

    public void OnUpdate(ref SystemState state)
    {
        Entity waveEntity = SystemAPI.GetSingletonEntity<GameState>();
        var gameState = state.EntityManager.GetComponentData<GameState>(waveEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (enemyStats, entity) in
            SystemAPI.Query<RefRO<EnemyStats>>()
                .WithAll<EnemyTag, DestroyTag>()
                .WithNone<ScoredTag>()
                .WithEntityAccess())
        {
            gameState.Score += enemyStats.ValueRO.ScoreValue;
            ecb.AddComponent<ScoredTag>(entity);
        }

        state.EntityManager.SetComponentData(waveEntity, gameState);
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}