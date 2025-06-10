using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Настройки восстановления")]
    [Tooltip("Сколько здоровья восстанавливает при подборе")]
    public float healAmount = 20f;

    [Tooltip("Время существования сферы (сек)")]
    public float lifetime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Восстанавливаем здоровье
            var player = PlayerController.Instance;
            player.currentHealth = Mathf.Min(
                player.currentHealth + healAmount,
                player.maxHealth
            );
            UIController.Instance.UpdateHealthSlider();

            // Уничтожаем сферу
            Destroy(gameObject);
        }
    }
}
