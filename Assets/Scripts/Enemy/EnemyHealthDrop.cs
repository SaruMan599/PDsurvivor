using UnityEngine;

public class EnemyHealthDrop : MonoBehaviour
{
    [Header("Шанс выпадения")]
    [Tooltip("Вероятность (0–1), что при смерти появится сфера")]
    [Range(0f, 1f)]
    public float dropChance = 0.1f; // 10% шанс

    [Header("Префаб сферы здоровья")]
    public GameObject healthPickupPrefab;

    // Этот метод должен быть вызван при смерти врага
    public void TryDropHealth()
    {
        if (Random.value <= dropChance && healthPickupPrefab != null)
        {
            Instantiate(
                healthPickupPrefab,
                transform.position,
                Quaternion.identity
            );
        }
    }
}
