using Unity.Entities;

public struct WaveManager : IComponentData
{
    public int CurrentWave;
    public int TotalWaves;
    public int EnemiesRemainingToSpawn;
    public int EnemiesAlive;
    public float SpawnTimer;
    public float SpawnInterval;
    public WaveState State;
}
public enum WaveState
{
    WaitingToStart,
    Spawning,
    InProgress,
    Completed,
    GameOver
}