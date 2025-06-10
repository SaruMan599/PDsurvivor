using UnityEngine;

public class WindBlade : Weapon
{
    [SerializeField] private GameObject bladePrefab; // сюда перетащите префаб «ветряного клинка»

    private float spawnCounter;

    private void Update()
    {
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0f)
        {
            // Сброс перезарядки
            spawnCounter = stats[weaponLevel].cooldown;

            // Спавним один клинок прямо из позиции игрока (или куда угодно, где висит этот скрипт)
            Vector3 spawnPos = transform.position;
            GameObject blade = Instantiate(bladePrefab, spawnPos, Quaternion.identity);
            blade.GetComponent<WindBladePrefab>().Initialize(this);
        }
    }
}
