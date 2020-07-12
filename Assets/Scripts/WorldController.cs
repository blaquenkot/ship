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

    private List<PowerUpType> PowerUpTypes = new List<PowerUpType> {PowerUpType.Acceleration, PowerUpType.Shield, PowerUpType.Shoot, PowerUpType.Torque };
    private float CreateEnemyCooldown = 1f;
    private float CreatePowerUpCooldown = 0.5f;

    void FixedUpdate()
    {
        CreateEnemyCooldown -= Time.deltaTime;
        if(CreateEnemyCooldown <= 0f)
        {
            Vector2 randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            if(Random.Range(0, 2) == 0)
            {
                Instantiate(Enemy1, randomPositionOnScreen, transform.rotation, transform.parent);
            } 
            else 
            {
                Instantiate(Enemy2, randomPositionOnScreen, transform.rotation, transform.parent);
            }
            CreateEnemyCooldown = 1.5f;
        }

        CreatePowerUpCooldown -= Time.deltaTime;
        if(CreatePowerUpCooldown <= 0f)
        {
            Vector2 randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            PowerUpType type = PowerUpTypes[Random.Range(0, PowerUpTypes.Count)];
            switch(type)
            {
                case PowerUpType.Acceleration:
                {
                    Instantiate(AccelerationPowerUp, randomPositionOnScreen, transform.rotation, transform.parent);
                    break;
                }
                case PowerUpType.Torque:
                {
                    Instantiate(RotationPowerUp, randomPositionOnScreen, transform.rotation, transform.parent);
                    break;
                }
                case PowerUpType.Shoot:
                {
                    Instantiate(BlastersPowerUp, randomPositionOnScreen, transform.rotation, transform.parent);
                    break;
                }
                case PowerUpType.Shield:
                {
                    Instantiate(ShieldPowerUp, randomPositionOnScreen, transform.rotation, transform.parent);
                    break;
                }
            }

            CreatePowerUpCooldown = 0.5f;
        }
    }
}