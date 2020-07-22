using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public GameObject Ship;
    public GameObject Asteroid;
    public GameObject Enemy1;
    public GameObject Enemy2;

    public GameObject AccelerationPowerUp;
    public GameObject RotationPowerUp;    
    public GameObject ShieldPowerUp;    
    public GameObject BlastersPowerUp;
    public GameObject GameOverObject;
    public GameObject UIObject;

    private GameOverController GameOverController;
    private UIController UIController;

    private List<PowerUpType> PowerUpTypes = new List<PowerUpType> { PowerUpType.Acceleration, PowerUpType.Shield, PowerUpType.Shoot, PowerUpType.Torque };
    private float CreateEnemyCooldown = 1f;
    private float CreatePowerUpCooldown = 0.5f;
    private float CreateAsteroidCooldown = 0.75f;

    private int Points = 0;
    private float TotalTime = 0;

    public void Awake()
    {
        GameOverController = GameOverObject.GetComponent<GameOverController>();
        UIController = UIObject.GetComponent<UIController>();
    }

    public void AddPoints(int points) 
    {
        Points += points;
        UIController.UpdatePoints(Points);
    }

    public void ShipDestroyed()
    {
        gameObject.SetActive(false);
        GameOverController.UpdateInfo(Points, GetTimeAsString());
        GameOverObject.SetActive(true);
    }

    void LateUpdate()
    {
        TotalTime += Time.deltaTime;
        UIController.UpdateTime(GetTimeAsString());

        CreateAsteroidCooldown -= Time.deltaTime;
        if(CreateAsteroidCooldown <= 0f)
        {
            Instantiate(Asteroid, GetRandomPosition(2f, 0), transform.rotation, transform.parent);

            CreateAsteroidCooldown = 0.75f;
        }

        CreateEnemyCooldown -= Time.deltaTime;
        if(CreateEnemyCooldown <= 0f)
        {
            Vector2 randomPositionOnScreen = GetRandomPosition(2f, 0);
            GameObject enemyPrefab = null;
            if(Random.Range(0, 2) == 0)
            {
                enemyPrefab = Enemy1;
            } 
            else 
            {
                enemyPrefab = Enemy2;
            }
            Instantiate(enemyPrefab, randomPositionOnScreen, transform.rotation, transform.parent);

            CreateEnemyCooldown = 1.5f;
        }

        CreatePowerUpCooldown -= Time.deltaTime;
        if(CreatePowerUpCooldown <= 0f)
        {
            Vector2 randomPositionOnScreen = GetRandomPosition(1f, 0);
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
            Instantiate(powerUpPrefab, randomPositionOnScreen, transform.rotation, transform.parent);

            CreatePowerUpCooldown = 0.5f;
        }
    }

    private Vector2 GetRandomPosition(float radius, int iteration)
    {
        iteration += 1;

        if(iteration >= 50) {
            return Vector2.positiveInfinity;
        }

        // 0.25 for screen left margin
        Vector2 location = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.25f, 1.0f), Random.value));

        if(Vector2.Distance(Ship.transform.position, location) > 7.5f) {
            transform.position = location;
    
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);
    
            if (hitColliders.Length == 0) {
                return location;
            }
            else 
            {
                return GetRandomPosition(radius, iteration);
            }
        } else {
            return GetRandomPosition(radius, iteration);
        }
    }

    private string GetTimeAsString()
    {
        return Mathf.Floor(TotalTime / 60).ToString("00") + ':' + (TotalTime % 60).ToString("00");
    }
}