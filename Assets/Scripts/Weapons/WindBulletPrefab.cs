using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WindBulletPrefab : MonoBehaviour
{
    private WindBullet weapon;
    private Transform target;
    private Rigidbody2D rb;

    [SerializeField] private float bulletSpeed = 10f; // 🔹 Уникальная скорость пули
    private float damage;

    public void Initialize(WindBullet weaponSource)
    {
        weapon = weaponSource;
        rb = GetComponent<Rigidbody2D>();

        damage = weapon.stats[weapon.weaponLevel].damage;
        target = FindClosestEnemy();

        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * bulletSpeed;

            // 🔸 Поворачиваем спрайт пули к цели
            float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 180);
        }

        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Ищем любой IDamageable (не только Enemy)
        IDamageable dmgable = collider.GetComponent<IDamageable>();
        if (dmgable != null)
        {
            // Рассчитываем урон и наносим его
            float playerAttack = PlayerController.Instance.baseAttack;
            float playerSpeed = PlayerController.Instance.moveSpeed;
            float totalDamage = (playerAttack + damage) * (playerSpeed / 2f);
            dmgable.TakeDamage(totalDamage);
            Destroy(gameObject);
        }
        else
        {
            // Если это стена/препятствие—можем просто уничтожить пулю
            if (collider.CompareTag("Environment") || collider.CompareTag("Obstacle"))
            {
                Destroy(gameObject);
            }
        }
    }

    private Transform FindClosestEnemy()
    {
        // Можно искать объекты с тегом "Enemy" или вообще всех с IDamageable
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject go in enemies)
        {
            float dist = Vector3.Distance(transform.position, go.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = go.transform;
            }
        }

        return closest;
    }
}
