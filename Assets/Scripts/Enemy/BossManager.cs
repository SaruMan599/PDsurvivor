// Assets/Scripts/Enemy/BossManager.cs
using UnityEngine;
using System.Collections.Generic;

public class BossManager : MonoBehaviour
{
    [Header("Настройки босса")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private int killThreshold = 500;
    [SerializeField] private Transform spawnPointRight;       // точка справа за экраном
    [SerializeField] private Transform spawnPointBelowPlayer;  // точка справа, ниже игрока
    [SerializeField] private Transform centerTopPoint;         // центр верхней половины

    private bool bossSpawned = false;
    private List<Enemy> disabledEnemies = new List<Enemy>();
    private BossController currentBoss;

    void Update()
    {
        if (!bossSpawned && UIController.Instance.totalKills >= killThreshold)
        {
            SpawnBoss();
            bossSpawned = true;
        }
    }

    private void SpawnBoss()
    {
        // 1. Отключаем всех врагов на сцене
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        disabledEnemies.Clear();
        foreach (var e in enemies)
        {
            disabledEnemies.Add(e);
            e.gameObject.SetActive(false);
        }

        // 2. Отключаем спавнер
        if (spawner != null)
            spawner.enabled = false;

        // 3. Создаем босса
        GameObject bossObj = Instantiate(bossPrefab, spawnPointRight.position, Quaternion.identity);
        currentBoss = bossObj.GetComponent<BossController>();
        currentBoss.Initialize(this, spawnPointRight.position, spawnPointBelowPlayer.position, centerTopPoint.position);
    }

    // Если босс убит
    public void OnBossDefeated()
    {
        int totalXP = 0;
        foreach (var e in disabledEnemies)
        {
            if (e != null)
                totalXP += e.experienceToGive;
        }

        // Уничтожаем отключенных врагов без выдачи опыта
        foreach (var e in disabledEnemies)
            if (e != null)
                Destroy(e.gameObject);
        disabledEnemies.Clear();

        // Возвращаем спавнер
        if (spawner != null)
            spawner.enabled = true;

        // Дарим игроку XP
        PlayerController.Instance.GetExperience(totalXP);

        // Создаем сундук на месте босса
        if (currentBoss != null && currentBoss.ChestPrefab != null)
        {
            Instantiate(currentBoss.ChestPrefab, currentBoss.transform.position, Quaternion.identity);
        }
    }

    // Если босс сбежал
    public void OnBossEscaped()
    {
        // Уничтожаем отключенных врагов без XP
        foreach (var e in disabledEnemies)
            if (e != null)
                Destroy(e.gameObject);
        disabledEnemies.Clear();

        // Возвращаем спавнер
        if (spawner != null)
            spawner.enabled = true;
    }
}
