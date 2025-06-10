// LocalizationManager.cs
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public enum Language { RU, EN }
    public Language CurrentLanguage { get; private set; }

    private Dictionary<string, string> ruDict = new Dictionary<string, string>()
    {
        // --- Артефакты и их описания ---
        { "LampOfGenie_Name",       "Лампа джина" },
        { "LampOfGenie_Text",       "+100 к атаке\n+20% к кр. урону\n-20 к макс. HP" },
        { "GrimoireEarth_Name",     "Гримуар школы земли" },
        { "GrimoireEarth_Text",     "-10% к получаемому урону\n+10% к макс HP" },
        { "LuckRainbow_Name",       "Удача радуги" },
        { "LuckRainbow_Text",       "+5% к размеру заклинаний" },
        { "SimurghFeather_Name",    "Перо Симурга" },
        { "SimurghFeather_Text",    "+20% шанс крита\n-10% к перезарядке\n+5% к радиусу подбора" },
        { "WindSpirit_Name",        "Малый дух ветра" },
        { "WindSpirit_Text",        "+20% к скорости\n+10% к уклонению" },
        { "WitchHat_Name",          "Шляпа ведьмы" },
        { "WitchHat_Text",          "+10% к атаке\n+5% к уменьшению перезарядки" },

        // --- UI-элементы и надписи ---
        { "Continue",      "Продолжить" },
        { "Cooldown",      "Перезарядка" },
        { "Duration",      "Длительность" },
        { "Damage",        "Урон" },
        { "Range",         "Дальность" },
        { "Speed",         "Скорость" },
        { "Amount",        "Количество" },
        { "Description",   "Описание" },

        { "Experience",    "Опыт" },
        { "Health",        "Здоровье" },

        { "GameOver",      "Игра окончена" },
        { "Restart",       "Повтор" },
        { "MainMenu",      "В меню" },

        { "Level Up",      "Уровень повышен"},
        { "WeaponName",    "Название оружия" },
        { "Choose an upgrade",  "Выберите улучшение"},

        { "SFX",           "Звуки" },
        { "Music",         "Музыка" },

        { "Save",          "Сохранить" },

        { "Artifact",      "Артефакт" },
        { "NameArtifact",      "Название Артефакта" },

        // --- Статы (небоевые) ---
        { "HealthStat",         "Здоровье" },
        { "RecoveryPerSec",     "Исцеление в сек." },
        { "DamageTaken",        "Получаемый урон" },
        { "Dodge",              "Уклонение" },
        { "SpeedStat",          "Скорость" },
        { "PickupRadius",       "Радиус сбора" },

        // --- Статы (боевые) ---
        { "AttackPower",        "Сила атаки" },
        { "AttackMultiplier",   "Преумножение атаки" },
        { "ProjectileSize",     "Размер заклинания" },
        { "CooldownStat",       "Перезарядка" },
        { "CritChance",         "Шанс крит. урона" },
        { "CritDamage",         "Крит. урон" },

        // --- Сводка сессии ---
        { "CurrentLevel",     "Текущий уровень:" },
        { "TimeAlive",        "Время жизни:" },
        { "EnemiesKilled",    "Уничтожено врагов:" },

        // --- Общие кнопки и панель ---
        { "Settings",        "Настройки" },
        { "Pause",           "Пауза" },
        { "RU",              "RU" },
        { "EN",              "EN" }
    };

    private Dictionary<string, string> enDict = new Dictionary<string, string>()
    {
        // --- Artifacts ---
        { "LampOfGenie_Name",       "Lamp of the Genie" },
        { "LampOfGenie_Text",       "+100 to attack\n+20% crit damage\n-20 to max HP" },
        { "GrimoireEarth_Name",     "Grimoire of Earth" },
        { "GrimoireEarth_Text",     "-10% damage taken\n+10% max HP" },
        { "LuckRainbow_Name",       "Luck of the Rainbow" },
        { "LuckRainbow_Text",       "+5% projectile size" },
        { "SimurghFeather_Name",    "Simurgh's Feather" },
        { "SimurghFeather_Text",    "+20% crit chance\n-10% cooldown\n+5% pickup radius" },
        { "WindSpirit_Name",        "Lesser Wind Spirit" },
        { "WindSpirit_Text",        "+20% speed\n+10% dodge chance" },
        { "WitchHat_Name",          "Witch's Hat" },
        { "WitchHat_Text",          "+10% to attack\n+5% cooldown reduction" },

        // --- UI labels ---
        { "Continue",      "Continue" },
        { "Cooldown",      "Cooldown" },
        { "Duration",      "Duration" },
        { "Damage",        "Damage" },
        { "Range",         "Range" },
        { "Speed",         "Speed" },
        { "Amount",        "Amount" },
        { "Description",   "Description" },

        { "Experience",    "Experience" },
        { "Health",        "Health" },

        { "GameOver",      "Game Over" },
        { "Restart",       "Restart" },
        { "MainMenu",      "Main Menu" },

        { "Level Up",       "Level Up"},
        { "WeaponName",    "Weapon Name" },
        { "Choose an upgrade", "Choose an upgrade"},

        { "SFX",           "SFX" },
        { "Music",         "Music" },

        { "Save",          "Save" },
        { "NameArtifact",      "Name Artifact" },
        { "Artifact",      "Artifact" },

        // --- Stats (non-combat) ---
        { "HealthStat",         "Health" },
        { "RecoveryPerSec",     "Recovery/sec" },
        { "DamageTaken",        "Damage Taken" },
        { "Dodge",              "Dodge" },
        { "SpeedStat",          "Speed" },
        { "PickupRadius",       "Pickup Radius" },

        // --- Stats (combat) ---
        { "AttackPower",        "Attack Power" },
        { "AttackMultiplier",   "Attack Multiplier" },
        { "ProjectileSize",     "Projectile Size" },
        { "CooldownStat",       "Cooldown" },
        { "CritChance",         "Crit Chance" },
        { "CritDamage",         "Crit Damage" },

        // --- Session summary ---
        { "CurrentLevel",     "Current Level:" },
        { "TimeAlive",        "Time Alive:" },
        { "EnemiesKilled",    "Enemies Killed:" },

        // --- Common buttons/panels ---
        { "Settings",        "Settings" },
        { "Pause",           "Pause" },
        { "RU",              "RU" },
        { "EN",              "EN" }
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        string lang = PlayerPrefs.GetString("Language", "RU");
        SetLanguage(lang);
    }

    public void SetLanguage(string langCode)
    {
        if (langCode == "RU") CurrentLanguage = Language.RU;
        else CurrentLanguage = Language.EN;

        PlayerPrefs.SetString("Language", langCode);
        PlayerPrefs.Save();

        foreach (var loc in FindObjectsOfType<LocalizedText>())
        {
            loc.UpdateText();
        }
    }

    public string GetLocalizedValue(string key)
    {
        if (CurrentLanguage == Language.RU && ruDict.ContainsKey(key))
            return ruDict[key];
        else if (CurrentLanguage == Language.EN && enDict.ContainsKey(key))
            return enDict[key];
        else
            return key;
    }
}
