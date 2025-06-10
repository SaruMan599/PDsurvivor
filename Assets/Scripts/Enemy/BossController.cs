// Assets/Scripts/Enemy/BossController.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour, IDamageable
{
    public GameObject ChestPrefab; // префаб сундука, присвоить в Inspector

    [Header("Настройки босса")]
    [SerializeField] private float maxHealth = 2000f;
    [SerializeField] private float baseSpeed = 3f;               // скорость в фазе 1
    [SerializeField] private float shootCooldownNormal = 0.5f;   // темп стрельбы в фазе 1
    [SerializeField] private float shootCooldownPanic = 0.3f;    // темп стрельбы в панике
    [SerializeField] private float shieldHealth = 500f;          // прочность щита во 2 фазе
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float trailInterval = 0.2f;         // интервал спавна следа

    private float currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BossManager manager;
    private Transform player;
    private int passCount = 0;
    private float speed;
    private float shootTimer = 0f;
    private bool shieldActive = false;
    private float shieldHP;
    private GameObject shieldObj;
    private enum State { Phase1, Phase2, Stunned, Panic, Escaped, Dead }
    private State state = State.Phase1;
    private Vector3 spawnPos1, spawnPos2, centerPos;
    private List<Transform> stormBullets = new List<Transform>();
    private float stormRadius = 5f;
    private float stormShrinkSpeed = 0.5f;
    private int stormIndex = 0;

    public void Initialize(BossManager mgr, Vector3 p1, Vector3 p2, Vector3 center)
    {
        manager = mgr;
        spawnPos1 = p1;
        spawnPos2 = p2;
        centerPos = center;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = PlayerController.Instance.transform;
        currentHealth = maxHealth;
        shieldHP = shieldHealth;
        speed = baseSpeed;
        transform.localScale *= 1.5f;
        spriteRenderer.color = new Color(1f, 0f, 1f); // розово-синий
        StartCoroutine(Phase1Behavior());
        StartCoroutine(SpawnTrail());
    }

    private IEnumerator Phase1Behavior()
    {
        state = State.Phase1;
        Vector3 target = new Vector3(
            -Camera.main.orthographicSize * Screen.width / Screen.height - 1,
            spawnPos1.y, 0);
        shootTimer = shootCooldownNormal;

        while (passCount < 2 && state == State.Phase1)
        {
            // Движемся влево и стреляем
            rb.linearVelocity = Vector3.left * speed;
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootCooldownNormal;
            }
            if (transform.position.x <= target.x)
            {
                passCount++;
                if (passCount < 2)
                {
                    speed = baseSpeed * 2f;
                    transform.position = spawnPos2;
                    target = new Vector3(
                        -Camera.main.orthographicSize * Screen.width / Screen.height - 1,
                        spawnPos2.y, 0);
                }
                else break;
            }
            yield return null;
        }

        // Переход во вторую фазу
        rb.linearVelocity = Vector2.zero;
        transform.position = centerPos;
        StartCoroutine(Phase2Behavior());
    }

    private IEnumerator Phase2Behavior()
    {
        state = State.Phase2;
        // Активируем щит
        shieldObj = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
        shieldActive = true;

        // Создаем пулю-кольцо
        int count = 12;
        for (int i = 0; i < count; i++)
        {
            float angle = i * 360f / count * Mathf.Deg2Rad;
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * stormRadius;
            GameObject b = Instantiate(bulletPrefab, pos, Quaternion.identity);
            stormBullets.Add(b.transform);
        }

        // Волна пулей
        while (state == State.Phase2)
        {
            if (shieldActive)
            {
                stormRadius -= stormShrinkSpeed * Time.deltaTime;
                for (int i = 0; i < stormBullets.Count; i++)
                {
                    Vector3 dir = (stormBullets[i].position - transform.position).normalized;
                    stormBullets[i].position = transform.position + dir * stormRadius;
                }
                Transform btr = stormBullets[stormIndex];
                Vector3 dirToPlayer = (player.position - btr.position).normalized;
                btr.GetComponent<Rigidbody2D>().linearVelocity = dirToPlayer * 8f;
                stormIndex = (stormIndex + 1) % stormBullets.Count;
                yield return new WaitForSeconds(0.2f);
            }
            else break;
        }

        // Щит сломан
        Destroy(shieldObj);
        state = State.Stunned;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(10f);

        if (currentHealth > 0)
        {
            state = State.Panic;
            StartCoroutine(PanicBehavior());
        }
    }

    private IEnumerator PanicBehavior()
    {
        float panicTimer = 10f;
        while (panicTimer > 0f && currentHealth > 0)
        {
            Vector3 dir = (transform.position - player.position).normalized;
            rb.linearVelocity = dir * baseSpeed;
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootCooldownPanic;
            }
            panicTimer -= Time.deltaTime;
            yield return null;
        }

        if (currentHealth > 0)
        {
            state = State.Escaped;
            manager.OnBossEscaped();
            Destroy(gameObject);
        }
    }

    private IEnumerator SpawnTrail()
    {
        while (state == State.Phase1)
        {
            Instantiate(trailPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(trailInterval);
        }
    }

    private void Shoot()
    {
        GameObject proj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector3 dir = (player.position - transform.position).normalized;
        proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 8f;
    }

    public void TakeDamage(float amount)
    {
        if (state == State.Phase2 && shieldActive)
        {
            float reduced = amount * 0.2f;
            shieldHP -= reduced;
            if (shieldHP <= 0f) shieldActive = false;
        }
        else if (state != State.Escaped && state != State.Dead)
        {
            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                state = State.Dead;
                manager.OnBossDefeated();
                Destroy(gameObject);
            }
        }
    }
}
