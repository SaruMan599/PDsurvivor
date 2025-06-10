// DamageNumberController.cs
using UnityEngine;

public class DamageNumberController : MonoBehaviour
{
    public static DamageNumberController Instance;
    public DamageNumber prefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    /// <summary>
    /// Вызывается при необходимости показать цифру урона.
    /// Если объект DamageNumber уже существует рядом с данной локацией, 
    /// мы просто добавляем к нему значение, иначе создаём новый.
    /// </summary>
    public void CreateNumber(float value, Vector3 location)
    {
        // Попробуем найти уже существующий DamageNumber поблизости (в радиусе 0.5f)
        Collider2D[] hits = Physics2D.OverlapCircleAll(location, 0.5f);
        foreach (var hit in hits)
        {
            var existing = hit.GetComponent<DamageNumber>();
            if (existing != null)
            {
                existing.SetValue(Mathf.RoundToInt(value));
                return;
            }
        }

        // Если подходящего нет — создаём новый
        DamageNumber dmgNum = Instantiate(prefab, location, Quaternion.identity, transform);
        dmgNum.SetValue(Mathf.RoundToInt(value));
    }
}
