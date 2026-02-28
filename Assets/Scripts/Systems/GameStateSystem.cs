using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(DamageApplySystem))]
public partial struct GameStateSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CastleTag>();
        state.RequireForUpdate<GameState>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // Check castle HP
        foreach (var (health, castleTag) in
            SystemAPI.Query<RefRO<Health>, RefRO<CastleTag>>())
        {
            if (health.ValueRO.CurrentHealth <= 0f)
            {
                Entity waveEntity = SystemAPI.GetSingletonEntity<GameState>();
                var gameState = state.EntityManager.GetComponentData<GameState>(waveEntity);

                if (!gameState.IsGameOver)
                {
                    gameState.IsGameOver = true;
                    gameState.IsVictory = false;
                    state.EntityManager.SetComponentData(waveEntity, gameState);

                    // Set wave state to GameOver
                    var waveManager = state.EntityManager.GetComponentData<WaveManager>(waveEntity);
                    waveManager.State = WaveState.GameOver;
                    state.EntityManager.SetComponentData(waveEntity, waveManager);
                }
            }
        }

        // Update score from killed enemies
        Entity waveEnt = SystemAPI.GetSingletonEntity<GameState>();
        var gs = state.EntityManager.GetComponentData<GameState>(waveEnt);

        foreach (var (enemyStats, destroyTag) in
            SystemAPI.Query<RefRO<EnemyStats>, RefRO<DestroyTag>>()
                .WithAll<EnemyTag>())
        {
            // Note: this runs every frame for tagged entities.
            // We need a "scored" tag to avoid double counting.
            // For MVP simplicity, score is added once in DestroySystem.
        }

        state.EntityManager.SetComponentData(waveEnt, gs);
    }
}