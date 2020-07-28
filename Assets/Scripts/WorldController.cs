using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class WorldController : MonoBehaviour
{
    public GameObject Ship;
    public Volume Volume;

    public GameObject Orb;
    public GameObject Arrow;

    public GameObject Asteroid;
    public GameObject Enemy1;
    public GameObject Enemy2;

    public GameObject AccelerationPowerUp;
    public GameObject RotationPowerUp;    
    public GameObject ShieldPowerUp;    
    public GameObject BlastersPowerUp;
    public GameObject GameOverObject;
    public GameObject YouWonObject;

    public GameObject UIObject;

    private GameOverController GameOverController;
    public UIController UIController;
    private ShipController ShipController;
    private ColorAdjustments ColorAdjustments;
    private List<GameObject> InactiveObjectsToActiveOnFirstOrb = new List<GameObject>();

    private List<PowerUpType> PowerUpTypes = new List<PowerUpType> { PowerUpType.Acceleration, PowerUpType.Shield, PowerUpType.Shoot, PowerUpType.Torque };
    private float CreateEnemyCooldown = 1f;
    private float CreatePowerUpCooldown = 0.5f;
    private float CreateAsteroidCooldown = 0.75f;
    private bool ShouldSpawnObjects = true;

    private int Orbs = 0;
    private int Points = 0;
    private float TotalTime = 0;

    private int Flashes = 0;
    private float FlashCooldown = 0f;

    public void Awake()
    {
        Volume.sharedProfile.TryGet<ColorAdjustments>(out ColorAdjustments);
        GameOverController = GameOverObject.GetComponent<GameOverController>();
        UIController = UIObject.GetComponent<UIController>();
        ShipController = Ship.GetComponent<ShipController>();

        for (int i = 0; i < 5; i++)
        {
            GameObject orb = Instantiate(Orb, Random.insideUnitCircle.normalized * Random.Range(50f, 150f), transform.rotation, transform.parent);
            GameObject arrow = Instantiate(Arrow, Vector2.zero, transform.rotation, transform.parent);
            arrow.GetComponent<ArrowController>().Target = orb;
            
            if(i != 0)
            {
                arrow.SetActive(false);
                InactiveObjectsToActiveOnFirstOrb.Add(arrow);
                orb.SetActive(false);
                InactiveObjectsToActiveOnFirstOrb.Add(orb);
            }
        }
    }

    public void Flash(int amount)
    {
        Flashes = amount;
        FlashCooldown = 0f;
    }

    public void AddPoints(int points) 
    {
        Points += points;
        UIController.UpdatePoints(Points);
    }

    public void EnemyKilled(bool wasSpecialAttack)
    {
        ShipController.EnemyKilled(wasSpecialAttack);
    }

    public void OrbPickedUp()
    {
        if(Orbs == 0)
        {
            foreach (var inactiveObject in InactiveObjectsToActiveOnFirstOrb)
            {
                inactiveObject.SetActive(true);
            }
        }

        Orbs += 1;
        AddPoints(1000);

        if (Orbs >= 5)
        {
            AllOrbsPickedUp();
        }
    }

    public void AllOrbsPickedUp()
    {
        ShouldSpawnObjects = false;
        StartCoroutine(ShowYouWon());
    }

    IEnumerator ShowYouWon()
    {
        yield return new WaitForSeconds(0.75f);
        YouWonObject.SetActive(true);
    }

    public void ShipDestroyed()
    {
        ShouldSpawnObjects = false;
        StartCoroutine(ShowGameOver());
    }

    IEnumerator ShowGameOver()
    {
        GameOverController.UpdateInfo(Points, GetTimeAsString());
        yield return new WaitForSeconds(0.75f);
        GameOverObject.SetActive(true);
    }

    void Update()
    {
        if(Flashes > 0)
        {
            FlashCooldown -= Time.deltaTime;

            if(FlashCooldown <= 0) 
            {

                if(ColorAdjustments.postExposure.value == 0f)
                {
                    ColorAdjustments.postExposure.SetValue(new NoInterpMinFloatParameter(10f, 0, true));
                }
                else
                {
                    ColorAdjustments.postExposure.SetValue(new NoInterpMinFloatParameter(0f, 0, true));
                    Flashes -= 1;
                }

                FlashCooldown = 0.1f;
            }
        }
    }

    void LateUpdate()
    {
        if(ShouldSpawnObjects) 
        {
            TotalTime += Time.deltaTime;
            
            CreateAsteroidCooldown -= Time.deltaTime;
            if(CreateAsteroidCooldown <= 0f)
            {
                Vector2? position = GetRandomPosition(2f, false, 0);
                if(position.HasValue) 
                {
                    Instantiate(Asteroid, position.Value, transform.rotation, transform.parent);
                }

                CreateAsteroidCooldown = 0.5f + Random.value;
            }

            CreateEnemyCooldown -= Time.deltaTime;
            if(CreateEnemyCooldown <= 0f)
            {
                Vector2? position = GetRandomPosition(2f, true, 0);
                if(position.HasValue) 
                {
                    GameObject enemyPrefab = null;
                    if(Random.Range(0, 2) == 0)
                    {
                        enemyPrefab = Enemy1;
                    } 
                    else 
                    {
                        enemyPrefab = Enemy2;
                    }
                    Instantiate(enemyPrefab, position.Value, transform.rotation, transform.parent);
                }
                CreateEnemyCooldown = 0.65f + Random.value;
            }

            CreatePowerUpCooldown -= Time.deltaTime;
            if(CreatePowerUpCooldown <= 0f)
            {
                Vector2? position = GetRandomPosition(1f, true, 0);
                if(position.HasValue) 
                {
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
                    Instantiate(powerUpPrefab, position.Value, transform.rotation, transform.parent);
                }
                CreatePowerUpCooldown = 0.2f + Random.value;
            }
        }
    }

    private Vector2? GetRandomPosition(float radius, bool inScreen, int iteration)
    {
        iteration += 1;

        if(iteration >= 50)
        {
            return null;
        }

        // 0.25 for screen left margin
        Vector2 location = Camera.main.ViewportToWorldPoint(GetRandomRelativePosition(inScreen));

        if(Vector2.Distance(Ship.transform.position, location) > 7.5f) {
            transform.position = location;
    
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);
    
            if (hitColliders.Length == 0) {
                return location;
            }
            else 
            {
                return GetRandomPosition(radius, inScreen, iteration);
            }
        } else {
            return GetRandomPosition(radius, inScreen, iteration);
        }
    }

    private Vector2 GetRandomRelativePosition(bool inScreen) 
    {
        if(inScreen) 
        {
            return new Vector2(Random.Range(0.3f, 1.0f), Random.value);
        }
        else 
        {
            float randomX = 0f;
            float randomY = 0f;

            if(Random.Range(0, 2) == 0)
            {
                randomX = Random.Range(0.05f, 0.25f);
            } 
            else 
            {
                randomX = Random.Range(1.05f, 1.25f);
            }

            if(Random.Range(0, 2) == 0)
            {
                randomY = Random.Range(-0.25f, -0.05f);
            } 
            else 
            {
                randomY = Random.Range(1.05f, 1.25f);
            }

            return new Vector2(randomX, randomY);
        }
    }
    private string GetTimeAsString()
    {
        return Mathf.Floor(TotalTime / 60).ToString("00") + ':' + (TotalTime % 60).ToString("00");
    }
}