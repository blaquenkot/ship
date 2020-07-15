using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public GameObject Enemy1;
    public GameObject Enemy2;

    public GameObject AccelerationPowerUp;
    public GameObject RotationPowerUp;    
    public GameObject ShieldPowerUp;    
    public GameObject BlastersPowerUp;

    private List<GameObject> Objects = new List<GameObject>();

    private List<PowerUpType> PowerUpTypes = new List<PowerUpType> {PowerUpType.Acceleration, PowerUpType.Shield, PowerUpType.Shoot, PowerUpType.Torque };
    private float CreateEnemyCooldown = 1f;
    private float CreatePowerUpCooldown = 0.5f;

    void FixedUpdate()
    {
        CreateEnemyCooldown -= Time.deltaTime;
        if(CreateEnemyCooldown <= 0f)
        {
            Vector2 randomPositionOnScreen = GetRandomPosition(2f);
            GameObject enemyPrefab = null;
            if(Random.Range(0, 2) == 0)
            {
                enemyPrefab = Enemy1;
            } 
            else 
            {
                enemyPrefab = Enemy2;
            }
            GameObject enemy = Instantiate(enemyPrefab, randomPositionOnScreen, transform.rotation, transform.parent);
            Objects.Add(enemy);

            CreateEnemyCooldown = 1.5f;
        }

        CreatePowerUpCooldown -= Time.deltaTime;
        if(CreatePowerUpCooldown <= 0f)
        {
            Vector2 randomPositionOnScreen = GetRandomPosition(1f);
            PowerUpType type = PowerUpTypes[Random.Range(0, PowerUpTypes.Count)];
            GameObject powerUpPrefab = null;
            switch(type)
            {
                case PowerUpType.Acceleration:
                {
                    powerUpPrefab = AccelerationPowerUp;
                    break;
                }
                case PowerUpType.Torque:
                {
                    powerUpPrefab = RotationPowerUp;
                    break;
                }
                case PowerUpType.Shoot:
                {
                    powerUpPrefab = BlastersPowerUp;
                    break;
                }
                case PowerUpType.Shield:
                {
                    powerUpPrefab = ShieldPowerUp;
                    break;
                }
            }
            GameObject powerUp = Instantiate(powerUpPrefab, randomPositionOnScreen, transform.rotation, transform.parent);
            Objects.Add(powerUp);

            CreatePowerUpCooldown = 0.5f;
        }
    }

    private Vector2 GetRandomPosition(float radius)
    {
        Vector2 location = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
        transform.position = location;
 
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);
 
        if (hitColliders.Length == 0)
            return location;
        else
            return GetRandomPosition(radius);
    }
}