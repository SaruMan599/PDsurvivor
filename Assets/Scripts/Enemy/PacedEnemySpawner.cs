using UnityEngine;
using System.Collections.Generic;

public class WaveSpawnerWithPacing : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public int spawnCount; // Сколько врагов спавнится за один вызов
    }

    [Header("📈 Параметры Пейсинга")]
    public AnimationCurve pacingCurve; // От 0 до 1 — указывает напряжённость
    public float minInterval = 0.2f;   // Интервал при максимальном напряжении
    public float maxInterval = 1.5f;   // Интервал при минимальном напряжении

    [Header("🕒 Временные параметры")]
    private float spawnTimer;
    private float elapsedTime;

    [Header("⚔️ Волны врагов")]
    public List<Wave> waves;

    [Header("📍 Позиции спавна")]
    public Transform minPos;
    public Transform maxPos;

    void Update()
    {
        elapsedTime = Time.timeSinceLevelLoad;

        // Оцениваем текущую напряжённость по кривой
        float pacing = pacingCurve.Evaluate(elapsedTime);

        // Интерполируем интервал между спавнами
        float currentInterval = Mathf.Lerp(maxInterval, minInterval, pacing);
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentInterval)
        {
            spawnTimer = 0f;
            SpawnByPacing(pacing);
        }
    }

    void SpawnByPacing(float pacing)
    {
        int index;

        // До 60% напряжения — только слабые враги
        if (pacing < 0.6f)
        {
            index = 0;
        }
        else
        {
            // Смешанные волны: начиная с арчеров
            index = Mathf.Clamp(1, 0, waves.Count - 1);
        }

        if (index < waves.Count)
        {
            for (int i = 0; i < waves[index].spawnCount; i++)
            {
                Instantiate(waves[index].enemyPrefab, RandomSpawnPoint(), Quaternion.identity);
            }
        }
    }

    private Vector2 RandomSpawnPoint()
    {
        Vector2 spawnPoint;

        if (Random.value > 0.5f)
        {
            spawnPoint.x = Random.Range(minPos.position.x, maxPos.position.x);
            spawnPoint.y = Random.value > 0.5f ? minPos.position.y : maxPos.position.y;
        }
        else
        {
            spawnPoint.y = Random.Range(minPos.position.y, maxPos.position.y);
            spawnPoint.x = Random.value > 0.5f ? minPos.position.x : maxPos.position.x;
        }

        return spawnPoint;
    }
}
