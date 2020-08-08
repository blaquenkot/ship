using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class WorldController : MonoBehaviour
{
    private const int MaxPilots = 5;
    private const float IntroTime = 2.5f;

    public ShipController ShipController;
    public Volume Volume;
    public GameObject SpaceStation;
    public GameObject SpaceStationArrow;
    public Sprite SpaceStationArrowSprite;
    public GameObject Pilot;
    public GameObject Arrow;
    public GameObject Asteroid;
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject PersistentDataControllerPrefab;

    public GameObject AccelerationPowerUp;
    public GameObject RotationPowerUp;    
    public GameObject ShieldPowerUp;    
    public GameObject BlastersPowerUp;
    public GameObject YouWonObject;
    public Sprite[] PilotImages;
    public GameOverController GameOverController;
    public PauseMenuController PauseMenuController;
    public GameUIController GameUIController;
    public NetworkingController NetworkingController;
    private PersistentDataController PersistentDataController;
    private ColorAdjustments ColorAdjustments;
    private List<PilotController> ActivePilots = new List<PilotController>();
    private List<GameObject> InactiveObjectsToActivateOnFirstPilot = new List<GameObject>();
    private List<PowerUpType> PowerUpTypes = new List<PowerUpType> { PowerUpType.Acceleration, PowerUpType.Shield, PowerUpType.Shoot, PowerUpType.Torque };

    private bool AllPilotsArrowsShown = false;
    private bool ShouldUpdate = true;
    private float CreateEnemyCooldown = 1.25f + 1.75f*IntroTime;
    private float CreatePowerUpCooldown = 0.75f + 1.75f*IntroTime;
    private float CreateAsteroidCooldown = 1f + 1.75f*IntroTime;

    private int TotalPilots = MaxPilots;
    private int PickedUpPilots = 0;
    private int Points = 0;
    private float TotalTime = 0;

    private int Flashes = 0;
    private float FlashCooldown = 0f;

    public void Awake()
    {
        NetworkingController.NewSession((_) => {});

        PersistentDataController = Object.FindObjectOfType<PersistentDataController>();
        if(PersistentDataController == null) {
            PersistentDataController = Instantiate(PersistentDataControllerPrefab).GetComponent<PersistentDataController>();
        }
        GameOverController.GetComponentInChildren<NewHighscoreController>().PersistentDataController = PersistentDataController;
        YouWonObject.GetComponentInChildren<NewHighscoreController>().PersistentDataController = PersistentDataController;

        Volume.sharedProfile.TryGet<ColorAdjustments>(out ColorAdjustments);

        List<Sprite> Pilots = PilotImages.ToList();

        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < MaxPilots; i++)
        {
            bool badPosition = true;
            Vector2 position = Vector2.zero;
            while(badPosition)
            {
                badPosition = false;
                position = Random.insideUnitCircle.normalized;
                foreach (var point in points)
                {
                    if(Vector2.Distance(point, position) < 0.5f)
                    {
                        badPosition = true;
                        break;
                    }
                }
            }
            points.Add(position);
            GameObject pilot = Instantiate(Pilot, position * Random.Range(75f, 175f), transform.rotation, transform.parent);
            GameObject arrow = Instantiate(Arrow, Vector2.zero, transform.rotation, transform.parent);
            ArrowController arrowController = arrow.GetComponent<ArrowController>();
            arrowController.Target = pilot;
            int index = Random.Range(0, Pilots.Count);
            arrowController.SetCentralImage(Pilots[index]);
            Pilots.RemoveAt(index);
            
            PilotController pilotController = pilot.GetComponent<PilotController>();
            pilotController.WorldController = this;
            pilotController.ArrowController = arrowController;

            if(i != 0)
            {
                arrow.SetActive(false);
                InactiveObjectsToActivateOnFirstPilot.Add(arrow);
                pilot.SetActive(false);
                InactiveObjectsToActivateOnFirstPilot.Add(pilot);
            }
            else
            {
                pilot.GetComponentInChildren<CameraLookableObject>().ForceCamera(IntroTime, 0.75f);
                arrowController.Blink(2);
            }
        }

        SpaceStation.SetActive(false);
        ArrowController spaceStationArrowController = SpaceStationArrow.GetComponent<ArrowController>();
        spaceStationArrowController.Target = SpaceStation;
        spaceStationArrowController.SetCentralImage(SpaceStationArrowSprite);
        SpaceStationArrow.SetActive(false);
    }
    public void Flash(int amount)
    {
        Flashes = amount;
        FlashCooldown = 0f;
    }

    public void AddPoints(int points) 
    {
        Points += points;
        GameUIController.UpdatePoints(Points);
    }

    public void EnemyKilled(bool wasSpecialAttack)
    {
        ShipController.EnemyKilled(wasSpecialAttack);
    }

    public void PilotDied(PilotController pilot)
    {
        ActivePilots.Remove(pilot);

        GameUIController.UpdateCurrentTarget(MissionTargetState.Lost);

        ShowAllPilotsArrows();

        TotalPilots -= 1;

        PilotsChanged();
    }

    public void PilotPickedUp(PilotController pilot)
    {
        ActivePilots.Remove(pilot);

        GameUIController.UpdateCurrentTarget(MissionTargetState.Completed);

        ShowAllPilotsArrows();

        PickedUpPilots += 1;
        
        AddPoints(1000);

        PilotsChanged();
    }

    public void SpaceStationReached()
    {
        ShouldUpdate = false;
        GameUIController.MissionSucceed();
        ShowYouWon();
    }

    void AllPilotsPickedUp()
    {
        SpaceStation.transform.position = ShipController.gameObject.transform.position + (Vector3)Random.insideUnitCircle.normalized * Random.Range(50f, 150f);
        SpaceStation.SetActive(true);
        SpaceStationArrow.SetActive(true);
        SpaceStationArrow.GetComponent<ArrowController>().Blink(2);
    }

    void ShowYouWon()
    {
        YouWonObject.SetActive(true);
    }

    public void ShipDestroyed()
    {
        ShouldUpdate = false;
        GameUIController.MissionFailed();
        StartCoroutine(ShowGameOver());
    }

    IEnumerator ShowGameOver()
    {
        GameOverController.UpdateInfo(Points, TotalTime);
        yield return new WaitForSeconds(0.75f);
        GameOverController.gameObject.SetActive(true);
    }

    void Update()
    {
        if(!ShouldUpdate) 
        { 
            return; 
        }

        TotalTime += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Escape) && !GameOverController.gameObject.activeSelf && !YouWonObject.activeSelf)
        {
            if(Time.timeScale == 0f)
            {
                UnpauseGame();
            }
            else 
            {
                PauseGame();
            }
            return;
        }

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

        int activePilotsCount = ActivePilots.Count;
        if(activePilotsCount > 1)
        {
            Vector3 shipPosition = ShipController.transform.position;
            Dictionary<PilotController, float> distances = new Dictionary<PilotController, float>();
            foreach (var pilot in ActivePilots)
            {
                distances.Add(pilot, Vector3.Distance(pilot.transform.position, shipPosition));
            }
            
            int order = activePilotsCount;
            foreach (var pair in distances.OrderBy(kvp => kvp.Value))
            {
                pair.Key.ArrowController.UpdateSortingOrder(order);
                order -= 1;
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        PauseMenuController.gameObject.SetActive(true);
    }

    void UnpauseGame()
    {
        Time.timeScale = 1f;
        PauseMenuController.gameObject.SetActive(false);
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

        if(Vector2.Distance(ShipController.gameObject.transform.position, location) > 7.5f) {
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
    private void PilotsChanged()
    {
        if (PickedUpPilots >= TotalPilots)
        {
            AllPilotsPickedUp();
        }
    }

    private void ShowAllPilotsArrows()
    {
        if(!AllPilotsArrowsShown) 
        {
            foreach (var inactiveObject in InactiveObjectsToActivateOnFirstPilot)
            {
                inactiveObject.SetActive(true);

                ArrowController arrowController = inactiveObject.GetComponent<ArrowController>();
                if(arrowController)
                {
                    arrowController.Blink(2);
                }
                else 
                {
                    PilotController pilotController = inactiveObject.GetComponent<PilotController>();
                    if(pilotController)
                    {
                        ActivePilots.Add(pilotController);
                    }
                }
            }

            AllPilotsArrowsShown = true;
        }
    }
}