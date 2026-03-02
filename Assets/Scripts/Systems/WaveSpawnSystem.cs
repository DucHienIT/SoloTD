using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct WaveSpawnSystem : ISystem
{
    private Random _random;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WaveManager>();
        state.RequireForUpdate<EnemyPrefabReference>();
        _random = Random.CreateFromIndex(12345);
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // Get wave manager singleton
        Entity waveEntity = SystemAPI.GetSingletonEntity<WaveManager>();
        var waveManager = state.EntityManager.GetComponentData<WaveManager>(waveEntity);
        var gameState = state.EntityManager.GetComponentData<GameState>(waveEntity);
        var prefabRef = state.EntityManager.GetComponentData<EnemyPrefabReference>(waveEntity);

        if (gameState.IsGameOver) return;

        switch (waveManager.State)
        {
            case WaveState.WaitingToStart:
                StartNextWave(ref waveManager, ref gameState);
                break;

            case WaveState.Spawning:
                UpdateSpawning(ref state, ref waveManager, prefabRef.Value, dt);
                break;

            case WaveState.InProgress:
                CheckWaveComplete(ref state, ref waveManager);
                break;

            case WaveState.Completed:
                // Brief pause then next wave
                waveManager.SpawnTimer -= dt;
                if (waveManager.SpawnTimer <= 0f)
                {
                    if (waveManager.CurrentWave >= waveManager.TotalWaves)
                    {
                        gameState.IsVictory = true;
                        gameState.IsGameOver = true;
                        waveManager.State = WaveState.GameOver;
                    }
                    else
                    {
                        waveManager.State = WaveState.WaitingToStart;
                    }
                }
                break;
        }

        state.EntityManager.SetComponentData(waveEntity, waveManager);
        state.EntityManager.SetComponentData(waveEntity, gameState);
    }

    private void StartNextWave(ref WaveManager waveManager, ref GameState gameState)
    {
        waveManager.CurrentWave++;
        gameState.CurrentWaveNumber = waveManager.CurrentWave;

        // Scale difficulty: more enemies per wave
        int baseCount = 5;
        int extraPerWave = 3;
        waveManager.EnemiesRemainingToSpawn = baseCount + (waveManager.CurrentWave - 1) * extraPerWave;
        waveManager.EnemiesAlive = 0;
        waveManager.SpawnTimer = 0f;

        // Speed up spawns in later waves
        waveManager.SpawnInterval = math.max(0.2f, 0.8f - (waveManager.CurrentWave - 1) * 0.05f);

        waveManager.State = WaveState.Spawning;
    }

    private void UpdateSpawning(
        ref SystemState state,
        ref WaveManager waveManager,
        Entity enemyPrefab,
        float dt)
    {
        waveManager.SpawnTimer -= dt;

        if (waveManager.SpawnTimer <= 0f && waveManager.EnemiesRemainingToSpawn > 0)
        {
            // Spawn one enemy
            SpawnEnemy(ref state, enemyPrefab, waveManager.CurrentWave);
            waveManager.EnemiesRemainingToSpawn--;
            waveManager.EnemiesAlive++;
            waveManager.SpawnTimer = waveManager.SpawnInterval;
        }

        if (waveManager.EnemiesRemainingToSpawn <= 0)
        {
            waveManager.State = WaveState.InProgress;
        }
    }

    private void SpawnEnemy(ref SystemState state, Entity prefab, int waveNumber)
    {
        var entity = state.EntityManager.Instantiate(prefab);

        // Random lane position (X from -2 to 2, 5 lanes)
        int lane = _random.NextInt(0, 5);
        float laneX = -2f + lane * 1f;
        float spawnY = 6f; // top of screen in portrait

        // Randomize X slightly within lane
        float offsetX = _random.NextFloat(-0.3f, 0.3f);

        state.EntityManager.SetComponentData(entity, LocalTransform.FromPosition(
            new float3(laneX + offsetX, spawnY, 0f)
        ));

        // Scale HP with wave number
        if (state.EntityManager.HasComponent<Health>(entity))
        {
            var health = state.EntityManager.GetComponentData<Health>(entity);
            float hpMultiplier = 1f + (waveNumber - 1) * 0.3f;
            health.Max *= hpMultiplier;
            health.CurrentHealth = health.Max;
            state.EntityManager.SetComponentData(entity, health);
        }

        // Set lane
        if (state.EntityManager.HasComponent<LaneIndex>(entity))
        {
            state.EntityManager.SetComponentData(entity, new LaneIndex { Value = lane });
        }
    }

    private void CheckWaveComplete(ref SystemState state, ref WaveManager waveManager)
    {
        // Count alive enemies
        int aliveCount = 0;
        foreach (var tag in SystemAPI.Query<RefRO<EnemyTag>>().WithNone<DestroyTag>())
        {
            aliveCount++;
        }

        waveManager.EnemiesAlive = aliveCount;

        if (aliveCount == 0)
        {
            waveManager.State = WaveState.Completed;
            waveManager.SpawnTimer = 2f; // pause between waves
        }
    }
}