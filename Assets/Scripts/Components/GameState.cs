using Unity.Entities;

public struct GameState : IComponentData
{
    public int Score;
    public int CurrentWaveNumber;
    public bool IsGameOver;
    public bool IsVictory;
}