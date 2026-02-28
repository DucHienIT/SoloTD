using UnityEngine;

[CreateAssetMenu(fileName = "HeroStatTable", menuName = "SoloTD/Hero Stat Table")]
public class HeroStatTableSO : ScriptableObject
{
    public HeroStatEntry[] Entries;
}

[System.Serializable]
public struct HeroStatEntry
{
    public string Name;
    public float MaxHP;
    public float AttackDamage;
    public float AttackRange;
    public float FireRate;
}