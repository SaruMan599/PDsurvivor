// ArtifactManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager Instance { get; private set; }

    [Header("Сундук")]
    public GameObject chestPrefab;
    public float chestSpawnInterval = 120f;
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 12f;

    [Header("Ссылка на UI-контроллер")]
    public ArtifactUIController uiController;

    [Header("Все артефакты (ScriptableObject)")]
    public ArtifactData[] allArtifactData;

    // Доступные (неподобранные) артефакты
    public List<ArtifactData> availableArtifacts =>
        allArtifactData
        .Where(a => !PlayerArtifactSystem.Instance.acquiredArtifacts.Contains(a))
        .ToList();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        if (chestPrefab == null) Debug.LogError("ArtifactManager: нужно назначить префаб сундука.");
        if (uiController == null) Debug.LogError("ArtifactManager: нужно назначить ArtifactUIController.");
        if (allArtifactData == null || allArtifactData.Length == 0)
            Debug.LogError("ArtifactManager: нужно добавить хотя бы один ArtifactData в инспекторе.");
    }

    private void Start()
    {
        StartCoroutine(SpawnChestsRoutine());
    }

    private IEnumerator SpawnChestsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(chestSpawnInterval);
            SpawnChestNearPlayer();
        }
    }

    private void SpawnChestNearPlayer()
    {
        if (PlayerController.Instance == null || !PlayerController.Instance.gameObject.activeSelf) return;

        Vector3 playerPos = PlayerController.Instance.transform.position;
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 spawnPos = playerPos + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * dist;

        Instantiate(chestPrefab, spawnPos, Quaternion.identity);
    }

    public void OnChestOpened()
    {
        var available = availableArtifacts;
        if (available.Count == 0)
        {
            Debug.Log("Все артефакты уже собраны.");
            return;
        }

        int index = Random.Range(0, available.Count);
        ArtifactData chosen = available[index];
        uiController.ShowArtifact(chosen);
    }
}
