using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider playerExperienceSlider;
    [SerializeField] private TMP_Text experienceText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject levelUpPanel;
    public GameObject joystick;
    [SerializeField] private TMP_Text timerText;

    public LevelUpButton[] levelUpButtons;

    [HideInInspector] public float gameTime;    // секунды с начала игры
    [HideInInspector] public int totalKills;    // счётчик убитых врагов

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Сколько времени проходит с начала текущей сессии
        gameTime += Time.deltaTime;
        UpdateTimerDisplay();
    }

    public void UpdateHealthSlider()
    {
        playerHealthSlider.maxValue = PlayerController.Instance.maxHealth;
        playerHealthSlider.value = PlayerController.Instance.currentHealth;
        healthText.text = $"{playerHealthSlider.value:F0} / {playerHealthSlider.maxValue:F0}";
    }

    public void UpdateExperienceSlider()
    {
        var pc = PlayerController.Instance;
        playerExperienceSlider.maxValue = pc.playerLevels[pc.currentLevel - 1];
        playerExperienceSlider.value = pc.experience;
        experienceText.text = $"{playerExperienceSlider.value} / {playerExperienceSlider.maxValue}";
    }

    public void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        timerText.text = $"{minutes}:{seconds:00}";
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        return $"{minutes}:{seconds:00}";
    }

    public void LevelUpPanelOpen()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LevelUpPanelClose()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
