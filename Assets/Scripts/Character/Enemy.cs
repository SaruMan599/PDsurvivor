using UnityEngine;

public abstract class Enemy : Character
{
    protected Transform player; // Ссылка на игрока

    protected Enemy(string name, int level, float maxHealth, float speed)
        : base(name, level, maxHealth, speed) { }

    protected virtual void Start()
    {
        // Ищем игрока
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("Игрок не найден! Убедитесь, что объект игрока имеет тег 'Player'.");
        }
    }

    // Общая логика движения (можно переопределить)
    protected abstract void Move();

    // Обновление поведения
    public virtual void UpdateBehavior()
    {
        if (Health > 0) Move();
    }

    protected override void Die()
    {
        Debug.Log($"{Name} уничтожен.");
        Destroy(gameObject);
    }
}
