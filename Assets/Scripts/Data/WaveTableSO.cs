using UnityEngine;

[CreateAssetMenu(fileName = "WaveTable", menuName = "SoloTD/Wave Table")]
public class WaveTableSO : ScriptableObject
{
    public WaveEntry[] Waves;
}

[System.Serializable]
public struct WaveEntry
{
    public int WaveNumber;
    public int EnemyCount;
    public float SpawnInterval;
    public int EnemyTypeIndex;     // index into EnemyStatTable
    public float HPMultiplier;     // scales enemy HP per wave
}