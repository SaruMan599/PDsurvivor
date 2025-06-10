using System.Collections.Generic;
using UnityEngine;

public class AreaWeaponPrefab : MonoBehaviour
{
    public AreaWeapon weapon;
    private Vector3 targetSize;
    private float timer;
    // Теперь храним список любых «раненых» существ, реализующих IDamageable
    public List<IDamageable> enemiesInRange = new List<IDamageable>(); 
    private float counter;
    private float rotationSpeed = 180f;
    private float currentZRotation = 0f;

    void Start()
    {
        weapon = GameObject.Find("Area Weapon").GetComponent<AreaWeapon>();
        targetSize = Vector3.one * weapon.stats[weapon.weaponLevel].range;
        transform.localScale = Vector3.zero;
        timer = weapon.stats[weapon.weaponLevel].duration;
        AudioController.Instance.PlaySound(AudioController.Instance.areaWeaponSpawn);
    }

    void Update()
    {
        //transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        // Вращение
        //transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        currentZRotation += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(60f, 0f, currentZRotation);
        // Рост и спад ауры
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime * 5);
        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            targetSize = Vector3.zero;
            if (transform.localScale.x <= 0.01f)
            {
                Destroy(gameObject);
                AudioController.Instance.PlaySound(AudioController.Instance.areaWeaponDespawn);
            }
        }

        // Периодический урон
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            counter = weapon.stats[weapon.weaponLevel].speed;
            float baseDamage = weapon.stats[weapon.weaponLevel].damage;
            float playerAttack = PlayerController.Instance.baseAttack;

            // Копируем список, чтобы безопасно итерироваться
            List<IDamageable> targets = new List<IDamageable>(enemiesInRange);

            foreach (IDamageable d in targets)
            {
                if (d == null) continue;

                float totalDamage = baseDamage + playerAttack;

                // Вызываем универсальный метод
                d.TakeDamage(totalDamage);

                // Дополнительный knockback: пытаемся получить Rigidbody2D, если он есть
                if (d is MonoBehaviour mb && mb.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                {
                    Vector2 knockbackDir = ((mb.transform.position - transform.position).normalized);
                    float knockbackForce = 5f;
                    rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Проверяем, реализует ли объект IDamageable
        IDamageable dmgable = collider.GetComponent<IDamageable>();
        if (dmgable != null && !enemiesInRange.Contains(dmgable))
        {
            enemiesInRange.Add(dmgable);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        IDamageable dmgable = collider.GetComponent<IDamageable>();
        if (dmgable != null && enemiesInRange.Contains(dmgable))
        {
            enemiesInRange.Remove(dmgable);
        }
    }
}
