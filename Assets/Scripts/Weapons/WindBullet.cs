using UnityEngine;

public class WindBullet : Weapon
{
    [SerializeField] private GameObject prefab;
    private float spawnCounter;

    private void Update()
    {
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = stats[weaponLevel].cooldown;

            int bulletCount = weaponLevel + 1; // увеличиваем с уровнем
            for (int i = 0; i < bulletCount; i++)
            {
                // Появление слева и справа от игрока
                Vector3 offset = new Vector3((i % 2 == 0 ? 1 : -1) * 0.5f * (i + 1), 0, 0);
                Instantiate(prefab, transform.position + offset, Quaternion.identity).GetComponent<WindBulletPrefab>().Initialize(this);
            }
        }
    }
}
