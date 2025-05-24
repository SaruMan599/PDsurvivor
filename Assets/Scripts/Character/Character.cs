using System.Collections.Generic;
using UnityEngine;
public abstract class Character : MonoBehaviour
{
    // Общие свойства для всех персонажей
    public string Name { get; set; } // Имя персонажа
    public int Level { get; protected set; } // Уровень персонажа
    public float Health { get; protected set; } // Текущее здоровье
    public float MaxHealth { get; protected set; } // Максимальное здоровье
    public float Speed { get; protected set; } // Скорость передвижения

    // Конструктор для базовой инициализации
    public Character(string name, int level, float maxHealth, float speed)
    {
        Name = name;
        Level = level;
        MaxHealth = maxHealth;
        Health = maxHealth; // Начальное здоровье — полное
        Speed = speed;
    }
    

    // Метод для получения урона (может быть переопределён в потомках)
    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die(); // Вызываем метод смерти
        }
    }

    // Метод для лечения
    public virtual void Heal(float amount)
    {
        Health = Mathf.Min(Health + amount, MaxHealth); // Не больше MaxHealth
    }

    // Абстрактный метод смерти (реализация у потомков)
    protected abstract void Die();
    

    // Метод передвижения (может быть переопределён для врагов или игрока)
    public virtual void Move(float horizontalInput, float verticalInput)
    {
        // Простая логика передвижения (можно доработать)
        float deltaX = horizontalInput * Speed * Time.deltaTime;
        float deltaY = verticalInput * Speed * Time.deltaTime;

        // Реализация перемещения зависит от движка, например, Unity
        // transform.Translate(new Vector3(deltaX, deltaY, 0));
    }

    // Интерфейс для получения статуса
    public virtual string GetStatus()
    {
        return $"{Name} (Level {Level}): {Health}/{MaxHealth} HP";
    }
}