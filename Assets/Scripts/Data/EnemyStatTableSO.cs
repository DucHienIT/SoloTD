using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatTable", menuName = "SoloTD/Enemy Stat Table")]
public class EnemyStatTableSO : ScriptableObject
{
    public EnemyStatEntry[] Entries;
}

[System.Serializable]
public struct EnemyStatEntry
{
    public string Name;
    public float MaxHP;
    public float MoveSpeed;
    public float DamageToCastle;
    public int ScoreValue;
}