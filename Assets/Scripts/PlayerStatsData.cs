// PlayerStatsData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerStatsData")]
public class PlayerStatsData : ScriptableObject
{
    [Header("Основные")]
    public float maxHealth = 100;
    public float recovery = 0f; // HP в секунду
    public float damageTakenMultiplier = 1f; // 1 = 100%
    public float dodgeChance = 0.05f; // 0.05 = 5%
    public float moveSpeed = 100f;
    public float pickupRange = 1.5f; // множитель

    [Header("Атака")]
    public float baseAttack = 100f;
    public float attackMultiplier = 1f; // 1.4 = +40%
    public float projectileSize = 1f; // 1 = 100%
    public float cooldownReduction = 0f; // 0.11 = -11%
    public float critChance = 0.4f;
    public float critMultiplier = 2.34f; // 2.34 = +134%
}
