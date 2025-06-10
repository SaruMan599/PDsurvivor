using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WindBladePrefab : MonoBehaviour
{
    private WindBlade weaponSource;
    private Rigidbody2D rb;
    private float lifeTimer = 3f;            // время жизни клинка
    private float elapsed = 0f;
    private Vector3 initialScale;            // начальный размер (берём из инспектора или = текущему)
    private Vector3 targetScale;             // конечный размер (в 3 раза больше)
    private float damage;                    // рассчитанный урон
    private float travelSpeed;               // скорость полёта

    public void Initialize(WindBlade source)
    {
        weaponSource = source;
        rb = GetComponent<Rigidbody2D>();

        // Запомним начальный и конечный масштаб (в 3 раза больше)
        initialScale = transform.localScale == Vector3.zero ? Vector3.one : transform.localScale;
        targetScale = initialScale * 3f;

        // Расчёт урона:
        // (атакa игрока + базовый урон клинка) * (1 + attackMultiplier игрока)
        float playerAttack = PlayerController.Instance.baseAttack;
        float playerMultiplier = PlayerController.Instance.attackMultiplier;
        float bladeBaseDamage = weaponSource.stats[weaponSource.weaponLevel].damage;
        damage = (playerAttack + bladeBaseDamage) * (1f + playerMultiplier);

        // Скорость полёта: сразу устанавливаем в travelSpeed (можно не менять из WeaponStats)
        travelSpeed = weaponSource.stats[weaponSource.weaponLevel].speed;

        // Найдём ближайшую цель (если есть). Если цели нет, клинок полетит вперёд по направлению «вперёд» (world‐вектор).
        Transform target = FindClosestEnemy();
        Vector2 direction;
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            // default: вперёд по Х (или Y) — можно скорректировать
            direction = transform.right; // пусть локальный «right» смотрит туда, куда вам нужно
        }

        // Устанавливаем скорость Rigidbody2D
        rb.linearVelocity = direction * travelSpeed;

        // Поворачиваем клинок в направлении движения
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Клинок живёт ровно lifeTimer секунд
        Destroy(gameObject, lifeTimer);
    }

    private void Update()
    {
        // Плавный рост: от initialScale до targetScale за lifeTimer секунд
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifeTimer);
        transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Наносим рассчитанный урон, но НЕ уничтожаем клинок (сквозная атака)
                enemy.TakeDamage(damage);

                // Здесь можно добавить лёгкий сдвиг врага (если нужно)
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    float knockbackForce = 2f; // мягкий отталкивающий эффект
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    private Transform FindClosestEnemy()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy e in allEnemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e.transform;
            }
        }

        return closest;
    }
}
