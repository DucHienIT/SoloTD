using Unity.Entities;
using UnityEngine;

public class WaveManagerAuthoring : MonoBehaviour
{
    public int TotalWaves = 10;
    public GameObject EnemyPrefab;

    public class Baker : Baker<WaveManagerAuthoring>
    {
        public override void Bake(WaveManagerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new WaveManager
            {
                CurrentWave = 0,
                TotalWaves = authoring.TotalWaves,
                EnemiesRemainingToSpawn = 0,
                EnemiesAlive = 0,
                SpawnTimer = 0f,
                SpawnInterval = 0.5f,
                State = WaveState.WaitingToStart
            });

            AddComponent(entity, new GameState
            {
                Score = 0,
                CurrentWaveNumber = 0,
                IsGameOver = false,
                IsVictory = false
            });

            if (authoring.EnemyPrefab != null)
            {
                AddComponent(entity, new EnemyPrefabReference
                {
                    Value = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}

// Extra component to hold the enemy prefab entity reference
public struct EnemyPrefabReference : IComponentData
{
    public Entity Value;
}