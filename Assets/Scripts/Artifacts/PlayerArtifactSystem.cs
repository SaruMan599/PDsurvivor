// PlayerArtifactSystem.cs
using UnityEngine;
using System.Collections.Generic;

public class PlayerArtifactSystem : MonoBehaviour
{
    public static PlayerArtifactSystem Instance { get; private set; }
    public List<ArtifactData> acquiredArtifacts = new List<ArtifactData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void AddArtifact(ArtifactData artifact)
    {
        if (acquiredArtifacts.Contains(artifact))
            return;

        acquiredArtifacts.Add(artifact);
        ApplyArtifactEffects(artifact);
        StatsDisplay.Instance.UpdateStats(); 
    }

    private void ApplyArtifactEffects(ArtifactData data)
    {
        var player = PlayerController.Instance;

        // Процентные бонусы
        player.baseAttack             *= 1f + data.attackPercentBonus / 100f;
        player.cooldownReduction      += data.cooldownReductionPercent / 100f;
        player.projectileSize         *= 1f + data.projectileSizePercent / 100f;
        player.moveSpeed              *= 1f + data.moveSpeedPercent / 100f;
        player.dodgeChance            += data.dodgeChancePercent / 100f;
        player.critChance             += data.critChancePercent / 100f;
        player.critMultiplier         += data.critDamagePercent / 100f;
        player.pickupRange            *= 1f + data.pickupRangePercent / 100f;
        player.maxHealth              *= 1f + data.maxHealthPercent / 100f;
        player.damageTakenMultiplier  *= 1f - data.damageReductionPercent / 100f;

        // Абсолютные значения
        player.baseAttack             += data.attackFlatBonus;
        player.maxHealth              -= data.maxHealthFlatPenalty;

        // Корректируем текущее здоровье, если оно выше нового максимума
        player.currentHealth = Mathf.Min(player.currentHealth, player.maxHealth);
        UIController.Instance.UpdateHealthSlider();
    }
}
