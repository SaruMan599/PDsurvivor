using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Scriptable Object")]
    public PlayerStatsData baseStats;

    [Header("Runtime Статы")]
    public float currentHealth;
    public float maxHealth;
    public float recovery;
    public float damageTakenMultiplier;
    public float dodgeChance;
    public float moveSpeed;//[SerializeField] private float moveSpeed;
    public float pickupRange;

    public float baseAttack;
    public float attackMultiplier;
    public float projectileSize;
    public float cooldownReduction;
    public float critChance;
    public float critMultiplier;

    [Header("Прочее")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private FixedJoystick joystick;
    public Vector3 playerMoveDirection;
    //
    public Vector3 lastMoveDirection;
    //
    public int experience;
    public int currentLevel;
    public int maxLevel;

    public List<int> playerLevels;
    //public Weapon activeWeapon;
    //
    [SerializeField] private List<Weapon> inactiveWeapons;
    public List<Weapon> activeWeapons;
    [SerializeField] private List<Weapon> upgradeableWeapons;
    public List<Weapon> maxLevelWeapons;
    //

    private bool isImmune;
    [SerializeField] private float immunityDuration;
    [SerializeField] private float immunityTimer;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        ApplyBaseStats();

        for (int i = playerLevels.Count; i < maxLevel; i++)
        {
            playerLevels.Add(Mathf.CeilToInt(playerLevels[playerLevels.Count - 1] * 1.1f + 15));
        }

        currentHealth = maxHealth;
        UIController.Instance.UpdateHealthSlider();
        UIController.Instance.UpdateExperienceSlider();
        AddWeapon(Random.Range(0, inactiveWeapons.Count));
    }

    void ApplyBaseStats()
    {
        maxHealth = baseStats.maxHealth;
        recovery = baseStats.recovery;
        damageTakenMultiplier = baseStats.damageTakenMultiplier;
        dodgeChance = baseStats.dodgeChance;
        moveSpeed = baseStats.moveSpeed;
        pickupRange = baseStats.pickupRange;

        baseAttack = baseStats.baseAttack;
        attackMultiplier = baseStats.attackMultiplier;
        projectileSize = baseStats.projectileSize;
        cooldownReduction = baseStats.cooldownReduction;
        critChance = baseStats.critChance;
        critMultiplier = baseStats.critMultiplier;
    }

    void Update()
    {
        float inputX = Mathf.Clamp(Input.GetAxisRaw("Horizontal") + joystick.Horizontal, -1f, 1f);
        float inputY = Mathf.Clamp(Input.GetAxisRaw("Vertical") + joystick.Vertical, -1f, 1f);
        playerMoveDirection = new Vector2(inputX, inputY).normalized;

        animator.SetFloat("moveX", inputX);
        animator.SetFloat("moveY", inputY);
        animator.SetBool("moving", playerMoveDirection != Vector3.zero);

        if (immunityTimer > 0) immunityTimer -= Time.deltaTime;
        else isImmune = false;

        // Автоматическое восстановление HP
        if (recovery > 0 && currentHealth < maxHealth)
        {
            currentHealth += recovery * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            UIController.Instance.UpdateHealthSlider();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(playerMoveDirection.x * moveSpeed, playerMoveDirection.y * moveSpeed);
    }//vector2

    public void TakeDamage(float damage)
    {
        if (!isImmune)
        {
            // Проверка на уклонение
            if (Random.value < dodgeChance)
            {
                // Уклонение успешно
                return;
            }

            isImmune = true;
            immunityTimer = immunityDuration;

            float finalDamage = damage * damageTakenMultiplier;//откуда берется мульти урон, возможно передается из расчета оружия
            currentHealth -= finalDamage;

            UIController.Instance.UpdateHealthSlider();

            if (currentHealth <= 0)
            {
                gameObject.SetActive(false);
                GameManager.Instance.GameOver();
            }
        }
    }

    public void GetExperience(int experienceToGet)
    {
        experience += experienceToGet;
        UIController.Instance.UpdateExperienceSlider();
        if (experience >= playerLevels[currentLevel - 1])
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        experience -= playerLevels[currentLevel - 1];
        currentLevel++;
        UIController.Instance.UpdateExperienceSlider();
        //UIController.Instance.levelUpButtons[0].ActivateButton(activeWeapon);

        upgradeableWeapons.Clear();

        if (activeWeapons.Count > 0)
        {
            upgradeableWeapons.AddRange(activeWeapons);
        }
        if (inactiveWeapons.Count > 0)
        {
            upgradeableWeapons.AddRange(inactiveWeapons);
        }
        for (int i = 0; i < UIController.Instance.levelUpButtons.Length; i++)
        {
            if (upgradeableWeapons.ElementAtOrDefault(i) != null)
            {
                UIController.Instance.levelUpButtons[i].ActivateButton(upgradeableWeapons[i]);
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(true);
            }
            else
            {
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(false);
            }
        }

        UIController.Instance.LevelUpPanelOpen();
    }

    private void AddWeapon(int index) {
        activeWeapons.Add(inactiveWeapons[index]);
        inactiveWeapons[index].gameObject.SetActive(true);
        inactiveWeapons.RemoveAt(index);
    }

    public void ActivateWeapon(Weapon weapon) {
        weapon.gameObject.SetActive(true);
        activeWeapons.Add(weapon);
        inactiveWeapons.Remove(weapon);
    }

    public void IncreaseMaxHelth(int value) {
        maxHealth += value;
        currentHealth = maxHealth;
        UIController.Instance.LevelUpPanelClose();
        //AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);

    }
        public void IncreaseMovementSpeed(float multiplier){
        moveSpeed *= multiplier;

        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }
}