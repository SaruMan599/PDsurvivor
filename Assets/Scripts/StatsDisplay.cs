using UnityEngine;
using TMPro;
using System;

public class StatsDisplay : MonoBehaviour
{
    [Header("UI-компоненты")]
    public TextMeshProUGUI nameStatsText;    // названия статов
    public TextMeshProUGUI valueStatsText;   // значения статов
    public TextMeshProUGUI sessionInfoText;  // сводка по сессии

    public static StatsDisplay Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        UpdateStats();
        UpdateSessionInfo();
    }

    public void UpdateStats()
    {
        var p = PlayerController.Instance;

        // Лейблы всех статов
        string labels =
            "Здоровье\n" +
            "Исцеление в сек.\n" +
            "Получаемый урон\n" +
            "Уклонение\n" +
            "Скорость\n" +
            "Радиус сбора\n" +
            "Сила атаки\n" +
            "Приумножение атаки\n" +
            "Размер заклинаний\n" +
            "Перезарядка\n" +
            "Шанс крит. урона\n" +
            "Крит. урон";

        // Значения соответствующих статов
        // Для перезарядки отображаем просто "-X%" (если есть снижение), иначе "0%"
        string cooldownDisplay = p.cooldownReduction > 0f
            ? $"-{(p.cooldownReduction * 100f):F0}%"
            : "0%";

        string values =
            $"{Mathf.RoundToInt(p.currentHealth)}/{Mathf.RoundToInt(p.maxHealth)}\n" +
            $"{(p.recovery * 100f):F0}%\n" +
            $"{(p.damageTakenMultiplier * 100f - 100f):+0;-0}%\n" +
            $"{p.dodgeChance:F0}%\n" +
            $"{p.moveSpeed:F0}\n" +
            $"{(p.pickupRange * 100f - 100f):+0;-0}%\n" +
            $"{p.baseAttack:F0}\n" +
            $"+{(p.attackMultiplier * 100f - 100f):F0}%\n" +
            $"+{(p.projectileSize * 100f - 100f):F0}%\n" +
            $"{cooldownDisplay}\n" +
            $"{(p.critChance * 100f):F0}%\n" +
            $"+{(p.critMultiplier * 100f):F0}%";

        nameStatsText.text  = labels;
        valueStatsText.text = values;
    }
    /*public void UpdateStats()
    {
        var p = PlayerController.Instance;
        var L = LocalizationManager.Instance;

        // Получаем локализованные названия по ключам
        string labels =
            $"{L.GetLocalizedValue("HealthStat")}\n" +
            $"{L.GetLocalizedValue("RecoveryPerSec")}\n" +
            $"{L.GetLocalizedValue("DamageTaken")}\n" +
            $"{L.GetLocalizedValue("Dodge")}\n" +
            $"{L.GetLocalizedValue("SpeedStat")}\n" +
            $"{L.GetLocalizedValue("PickupRadius")}\n" +
            $"{L.GetLocalizedValue("AttackPower")}\n" +
            $"{L.GetLocalizedValue("AttackMultiplier")}\n" +
            $"{L.GetLocalizedValue("ProjectileSize")}\n" +
            $"{L.GetLocalizedValue("CooldownStat")}\n" +
            $"{L.GetLocalizedValue("CritChance")}\n" +
            $"{L.GetLocalizedValue("CritDamage")}";

        // Значения статов (не локализуются, а подставляются цифры):
        string values =
            $"{Mathf.RoundToInt(p.currentHealth)}/{Mathf.RoundToInt(p.maxHealth)}\n" +
            $"{(p.recovery * 100f):F0}%\n" +
            $"{(p.damageTakenMultiplier * 100f - 100f):+0;-0}%\n" +
            $"{p.dodgeChance:F0}%\n" +
            $"{p.moveSpeed:F0}\n" +
            $"{(p.pickupRange * 100f - 100f):+0;-0}%\n" +
            $"{p.baseAttack:F0}\n" +
            $"+{(p.attackMultiplier * 100f - 100f):F0}%\n" +
            $"+{(p.projectileSize * 100f - 100f):F0}%\n" +
            $"-{(p.cooldownReduction * 100f - 100f):F0}%\n" +
            $"{(p.critChance * 100f):F0}%\n" +
            $"+{(p.critMultiplier * 100f):F0}%";

        nameStatsText.text = labels;
        valueStatsText.text = values;
    }*/


    public void UpdateSessionInfo()
    {
        float timeAlive = UIController.Instance.gameTime;
        string formatted = TimeSpan.FromSeconds(timeAlive).ToString(@"m\:ss");

        sessionInfoText.text =
            $"ПАУЗА\n\n" +
            $"Текущий уровень: {PlayerController.Instance.currentLevel}\n" +
            $"Время жизни: {formatted}\n" +
            $"Уничтожено врагов: {UIController.Instance.totalKills}";
    }
}
