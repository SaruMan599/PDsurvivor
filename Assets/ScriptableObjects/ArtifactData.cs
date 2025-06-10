// ArtifactData.cs
using UnityEngine;

public enum Rarity { Common, Rare, Epic }

[CreateAssetMenu(menuName = "Artifacts/ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    [TextArea] public string description;
    public Sprite icon;
    public Rarity rarity;

    [Header("Эффекты (в процентах / абсолютные значения)")]
    [Tooltip("+% к базовой атаке")]
    public float attackPercentBonus;            // например, 10 → +10% к baseAttack
    [Tooltip("% к уменьшению перезарядки")]
    public float cooldownReductionPercent;      // например, 5 → cooldownReduction += 0.05
    [Tooltip("% к размеру снарядов")]
    public float projectileSizePercent;         // 5 → projectileSize *= 1.05
    [Tooltip("% к скорости передвижения")]
    public float moveSpeedPercent;              // 20 → moveSpeed *= 1.20
    [Tooltip("% к шансу уклонения")]
    public float dodgeChancePercent;            // 10 → dodgeChance += 0.10
    [Tooltip("% к шансу крита")]
    public float critChancePercent;             // 20 → critChance += 0.20
    [Tooltip("% к критическому урону")]
    public float critDamagePercent;             // 2 → critMultiplier += 0.02
    [Tooltip("% к радиусу подбора предметов")]
    public float pickupRangePercent;            // 5 → pickupRange *= 1.05
    [Tooltip("% к макс. HP")]
    public float maxHealthPercent;              // 10 → maxHealth *= 1.10
    [Tooltip("% к уменьшению получаемого урона")]
    public float damageReductionPercent;        // 10 → damageTakenMultiplier *= 0.90

    [Header("Абсолютные значения")]
    [Tooltip("+ абсолютное к базовой атаке")]
    public float attackFlatBonus;                // например, +100 к baseAttack
    [Tooltip("- абсолютное к maxHP")]
    public float maxHealthFlatPenalty;           // например, -20 к maxHealth
}
