using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    [Header("Movement")]
    [SerializeField] private MovementController movementController;
    
    [Header("Health Settings")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private float damageEffectDuration = 0.2f;

    [Header("Experience Settings")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private int experiencePerLevelIncrease = 50;
    public int currentExperience { get; private set; }
    public int experienceToNextLevel { get; private set; }

    protected override void Start()
    {
        //base.Start();
        InitializeStats();
    }

    private void InitializeStats()
    {
        // Инициализация здоровья
        healthBar.maxValue = MaxHealth;
        healthBar.value = Health;

        // Инициализация опыта
        experienceToNextLevel = 100;
        xpBar.maxValue = experienceToNextLevel;
        xpBar.value = currentExperience;
    }

    public void TakeDamage(int damage)
    {
        //if (Health <= 0) return;

        Health = Mathf.Max(Health - damage, 0);
        healthBar.value = Health;
        StartCoroutine(DamageEffect());

        if (Health <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageEffect()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite == null) yield break;

        Color originalColor = sprite.color;
        sprite.color = Color.red;
        
        yield return new WaitForSeconds(damageEffectDuration);
        
        sprite.color = originalColor;
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        xpBar.value = currentExperience;

        if (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        currentExperience = 0;
        experienceToNextLevel += experiencePerLevelIncrease;
        xpBar.maxValue = experienceToNextLevel;
        xpBar.value = currentExperience;
        
        // Здесь можно добавить улучшение характеристик
    }

    protected override void Die()
    {
        Debug.Log("Player died!");
        movementController.enabled = false;
        // Дополнительные действия при смерти
    }
}