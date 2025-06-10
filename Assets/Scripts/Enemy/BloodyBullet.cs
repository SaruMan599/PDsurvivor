using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BloodyBullet : MonoBehaviour
{
    [Header("Настройки пули")]
    [SerializeField] private float bulletSpeed = 16f;   // Скорость полёта
    [SerializeField] private float lifetime = 3f;      // Время жизни пули
    [SerializeField] private float damage = 10f;       // Урон, наносимый при попадании

    private Rigidbody2D rb;
    private Transform playerTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = PlayerController.Instance.transform; // Ссылка на игрока

        Vector2 direction = Vector2.zero;
        if (playerTransform != null)
        {
            // Направляем пулю в сторону игрока в момент создания
            direction = (playerTransform.position - transform.position).normalized;
            float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 180);

        }
        else
        {
            direction = transform.right; // На всякий случай, если игрок не найден
        }

        rb.linearVelocity = direction * bulletSpeed;

        // Самоуничтожение через заданное время
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // Наносим урон игроку
            PlayerController.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
