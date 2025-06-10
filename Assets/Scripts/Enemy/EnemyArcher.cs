using UnityEngine;

public class EnemyArcher : MonoBehaviour, IDamageable
{
    //[Header("Спрятанные спрайты-круги (назначьте через инспектор)")]
    //[SerializeField] private SpriteRenderer innerCircleSprite;
    //[SerializeField] private SpriteRenderer outerCircleSprite;

    [Header("Настройки зон обнаружения")]
    [SerializeField] private float innerRadius = 7f;
    [SerializeField] private float outerRadius = 10f;
    [SerializeField] private float tolerance = 0.5f; // Увеличил допуск

    [Header("Скорости движения")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float holdDistanceSpeed = 1.5f;

    [Header("Настройки стрельбы")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float closeShootCooldown = 0.7f;

    [Header("Прочие настройки")]
    [SerializeField] private float health = 500f;
    [SerializeField] private int experienceToGive = 20;
    [SerializeField] private GameObject destroyEffect;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float shootTimer = 0f;

    [Header("Настройки избежания слипаний")]
    [SerializeField] private float avoidRadius = 1.5f;
    [SerializeField] private float avoidWeight = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        playerTransform = PlayerController.Instance.transform;
    }

    /*void Start()
    {
        // Визуализация через спрайты (если заданы)
        if (innerCircleSprite != null)
            innerCircleSprite.transform.localScale = Vector3.one * (innerRadius * 2f);
        if (outerCircleSprite != null)
            outerCircleSprite.transform.localScale = Vector3.one * (outerRadius * 2f);
    }*/

    void Update()
    {
        if (playerTransform == null || !PlayerController.Instance.gameObject.activeSelf)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("moving", false);
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 toPlayer = (playerTransform.position - transform.position).normalized;

        // 1. Вычисляем основной вектор mainDir и скорость currentSpeed
        Vector2 mainDir = Vector2.zero;
        float currentSpeed = 0f;

        // 1.1 Если игрок далеко (выше outerRadius) — приближаемся
        if (distance > outerRadius)
        {
            mainDir = toPlayer;
            currentSpeed = normalSpeed;
        }
        // 1.2 Если игрок внутри innerRadius — ОТХОДИМ (а не приближаемся!), но при этом стреляем быстрее
        else if (distance < innerRadius)
        {
            mainDir = -toPlayer;           // отходим от игрока
            currentSpeed = normalSpeed;
            TryShoot(closeShootCooldown);
        }
        // 1.3 Если игрок в «промежутке» — пытаемся встать на targetDistance ± tolerance
        else
        {
            float targetDistance = (innerRadius + outerRadius) / 2f;

            if (distance > targetDistance + tolerance)
            {
                // слишком далеко от идеальной точки — приближаемся, медленная скорость
                mainDir = toPlayer;
                currentSpeed = holdDistanceSpeed;
            }
            else if (distance < targetDistance - tolerance)
            {
                // слишком близко к идеальной точке — отступаем, медленная скорость
                mainDir = -toPlayer;
                currentSpeed = holdDistanceSpeed;
            }
            else
            {
                // внутри «мертвой зоны» (идеальная дистанция) — стоим и просто стреляем
                mainDir = Vector2.zero;
                currentSpeed = 0f;
            }
            TryShoot(shootCooldown);
        }

        // 2. Вычисляем вектор avoidDir для «скольжения» относительно союзников
        Vector2 avoidDir = Vector2.zero;
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject ally in allies)
        {
            if (ally == gameObject) continue;
            float d = Vector2.Distance(transform.position, ally.transform.position);
            if (d < avoidRadius && d > 0.01f)
            {
                Vector2 fromAlly = (transform.position - ally.transform.position).normalized;
                avoidDir += fromAlly * ((avoidRadius - d) / avoidRadius);
            }
        }

        // 3. Объединяем mainDir и avoidDir
        Vector2 combinedDir = mainDir;
        if (avoidDir.magnitude > 0.01f)
        {
            combinedDir = (mainDir + avoidDir * avoidWeight).normalized;
        }

        // 4. Устанавливаем скорость и анимацию
        if (combinedDir.magnitude > 0.01f)
        {
            rb.linearVelocity = combinedDir * currentSpeed;
            animator.SetBool("moving", true);
            animator.SetFloat("moveX", combinedDir.x);
            animator.SetFloat("moveY", combinedDir.y);

            // Переворот спрайта по оси X
            if (combinedDir.x > 0.1f) spriteRenderer.flipX = true;
            else if (combinedDir.x < -0.1f) spriteRenderer.flipX = false;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("moving", false);
        }
    }

    private void TryShoot(float cooldown)
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = cooldown;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);

        PlayerController.Instance.GetExperience(experienceToGive);
        Destroy(gameObject);
        UIController.Instance.totalKills++;
        GetComponent<EnemyHealthDrop>()?.TryDropHealth();
    }

    // Вариант дополнительной визуализации через Gizmos (работает в редакторе)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, innerRadius);

        Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        float targetDist = (innerRadius + outerRadius) / 2f;
        Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, targetDist);
    }
}
