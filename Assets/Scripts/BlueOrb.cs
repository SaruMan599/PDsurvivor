using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueOrb : MonoBehaviour
{
    [SerializeField] private int experiencePoints = 10; // Количество опыта, которое получает игрок

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверка, если объект, который соприкоснулся с BlueOrb, это игрок
        if (collision.CompareTag("Player"))
        {
            // Получение компонента Player у объекта
            Player player = collision.GetComponent<Player>();

            // Если у объекта есть компонент Player
            if (player != null)
            {
                player.GainExperience(experiencePoints); // Добавление опыта игроку
                Destroy(gameObject); // Удаление BlueOrb после получения опыта
            }
        }
    }
}
