// DamageNumber.cs
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private float showDelay = 0.5f;      // Задержка перед показом
    [SerializeField] private float displayDuration = 0.5f; // Время отображения после показа
    
    private float floatSpeed;
    private int damageValue;
    private float showAt;
    private bool isShown;

    void Start()
    {
        floatSpeed = Random.Range(0.1f, 1.5f);
        
        // Первоначально скрываем текст
        damageText.text = "";
        
        // Устанавливаем время, когда нужно показать цифры
        showAt = Time.time + showDelay;
        
        // Запланируем уничтожение через (showDelay + displayDuration) секунд
        Destroy(gameObject, showDelay + displayDuration);
    }

    void Update()
    {
        // Двигаем цифру вверх плавно
        transform.position += Vector3.up * Time.deltaTime * floatSpeed;

        // Показываем текст после задержки
        if (!isShown && Time.time >= showAt)
        {
            damageText.text = damageValue.ToString();
            isShown = true;
        }
    }

    /// <summary>
    /// Добавляем новое значение урона. Если цифры ещё не показаны — суммируем и сдвигаем время показа.
    /// </summary>
    public void SetValue(int value)
    {
        damageValue += value;
        showAt = Time.time + showDelay;

        // Сбрасываем таймер уничтожения и назначаем заново
        CancelInvoke(nameof(DestroyGameObject));
        Invoke(nameof(DestroyGameObject), showDelay + displayDuration);
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
