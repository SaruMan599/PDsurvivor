using UnityEngine;

public class MobArcher : Enemy
{
    [Header("Параметры моба")]
    [SerializeField] private string name = "MobArcherV1";
    [SerializeField] private int level = 1;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f; // Добавили кулдаун атак

    private float lastAttackTime;
    private Rigidbody2D rb;

    public MobArcher(string name, int level, float maxHealth, float speed) : base(name, level, maxHealth, speed)
    {
        
    }

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Отключаем гравитацию
        }
    }

    private void Update()
    {
        UpdateBehavior();
    }

    protected override void Move()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * Speed; // Используем физическое движение
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryAttackPlayer(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryAttackPlayer(collision.gameObject);
    }

    private void TryAttackPlayer(GameObject target)
    {
        if (target.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            Player playerScript = target.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
                lastAttackTime = Time.time;
                Debug.Log($"{name} атаковал игрока! Урон: {damage}");
            }
        }
    }
}