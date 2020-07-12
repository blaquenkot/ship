using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ShipController : MonoBehaviour, IDamageable
{
    public float AccelerationFactorMaxLimit = 8f;
    public float FactorMaxLimit = 2f;
    public float FactorMinLimit = 0f;
    public float HealthLimit = 0f;
    public float ShotCooldownTime = 0.5f;
    public float BaseShootPower = 10f;
    public GameObject LookAhead;
    public GameObject[] AccelerationParts;
    public GameObject AccelerationGaugeObject;
    public GameObject[] RotationParts;
    public GameObject RotationGaugeObject;
    public GameObject[] ShieldParts;
    public GameObject ShieldGaugeObject;
    public GameObject[] BlasterParts;
    public GameObject BlasterGaugeObject;
    public GameObject[] CannonsLevel1;
    public GameObject[] CannonsLevel2;
    public GameObject[] CannonsLevel3;
    public GameObject TailParticleObject;

    private Rigidbody2D Body;
    private IGauge AccelerationGauge;
    private IGauge RotationGauge;
    private IGauge ShieldGauge;
    private IGauge BlasterGauge;
    private ShakeCameraController ShakeCameraController;
    private CannonController[] CannonControllersLevel1;
    private CannonController[] CannonControllersLevel2;
    private CannonController[] CannonControllersLevel3;
    private ParticleSystem TailParticleSystem;

    private bool Shoot = false;
    private float DesiredRotation = 0f;
    private float DesiredMovement = 0f;

    private float ShotCooldown = 0f;
    private float AccelerationFactor = 4f;
    private float TorqueFactor = 1f;
    private float ShootFactor = 1f;
    private float ShieldFactor = 1f;
    private float AngularDragFactor = 1f;

    private float MovementDrag = 1f;

    private float BaseAngularDrag = 2f;
    private float BaseTorque = 500f;

    private AudioClip ShootSound;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        AccelerationGauge = AccelerationGaugeObject.GetComponent<IGauge>();
        RotationGauge = RotationGaugeObject.GetComponent<IGauge>();
        ShieldGauge = ShieldGaugeObject.GetComponent<IGauge>();
        BlasterGauge = BlasterGaugeObject.GetComponent<IGauge>();
        ShakeCameraController = UnityEngine.Object.FindObjectOfType<CinemachineVirtualCamera>().GetComponent<ShakeCameraController>();
        TailParticleSystem = TailParticleObject.GetComponent<ParticleSystem>();

        ShootSound = Resources.Load<AudioClip>("laser1");
        
        Body.angularDrag = BaseAngularDrag * AngularDragFactor;

        CannonControllersLevel1 = new CannonController[CannonsLevel1.Length];
        CannonControllersLevel2 = new CannonController[CannonsLevel2.Length];
        CannonControllersLevel3 = new CannonController[CannonsLevel3.Length];

        for (int i = 0; i < CannonsLevel1.Length; i++)
        {
            CannonControllersLevel1[i] = CannonsLevel1[i].GetComponent<CannonController>();
        }
        for (int i = 0; i < CannonsLevel2.Length; i++)
        {
            CannonControllersLevel2[i] = CannonsLevel2[i].GetComponent<CannonController>();
        }
        for (int i = 0; i < CannonsLevel3.Length; i++)
        {
            CannonControllersLevel3[i] = CannonsLevel3[i].GetComponent<CannonController>();
        }

        // Forcing update for ship components
        ModifyFactor(0f, PowerUpType.Acceleration);
        ModifyFactor(0f, PowerUpType.Shield);
        ModifyFactor(0f, PowerUpType.Shoot);
        ModifyFactor(0f, PowerUpType.Torque);
    }

    void Update()
    {
        DesiredRotation = Input.GetAxis("Horizontal");
        DesiredMovement = Mathf.Max(0f, Input.GetAxis("Vertical"));
        Shoot = Input.GetButton("Shoot");
        AccelerationGauge.SetValue(AccelerationFactor / AccelerationFactorMaxLimit);
        RotationGauge.SetValue(TorqueFactor / FactorMaxLimit);
        ShieldGauge.SetValue(ShieldFactor / FactorMaxLimit);
        BlasterGauge.SetValue(ShootFactor / FactorMaxLimit);
    }

    void FixedUpdate()
    {
        Vector2 direction = Vector2Utils.Vector2FromAngle(Body.rotation);

        if (DesiredMovement > 0) {
            // Set drag to give the ship a terminal velocity
            Body.drag = MovementDrag;

            Body.AddForce(
                DesiredMovement *
                    AccelerationFactor *
                    Time.deltaTime *
                    500.0f *
                    direction
            );

            if(TailParticleSystem.isStopped) 
            {
                TailParticleSystem.Play();
            }
        } else {
            Body.drag = 0;

            if(TailParticleSystem.isPlaying) 
            {
                TailParticleSystem.Stop();
            }
        }

        Body.AddTorque(
            -1 * BaseTorque * Time.deltaTime * DesiredRotation * TorqueFactor
        );

        ShotCooldown -= Time.deltaTime;
        if(ShotCooldown <= 0 && Shoot && ShootFactor > 0f)
        {
            if(BlasterParts[0].activeSelf) 
            {
                foreach (var cannon in CannonControllersLevel1)
                {
                    cannon.Fire(direction);
                }
            }
            if(BlasterParts[1].activeSelf) 
            {
                foreach (var cannon in CannonControllersLevel2)
                {
                    cannon.Fire(direction);
                }
            }
            if(BlasterParts[2].activeSelf) {
                foreach (var cannon in CannonControllersLevel3)
                {
                    cannon.Fire(direction);
                }
            }
            
            AudioSource.PlayClipAtPoint(ShootSound, transform.position);
            ShakeCameraController.Shake();

            ShotCooldown = ShotCooldownTime;
        }
    }

    public void TakeDamage(float damageTaken)
    {
        if(ShieldFactor > FactorMinLimit) {
            ModifyShieldFactor(-damageTaken);
        } 
        else 
        {
            List<PowerUpType> types = new List<PowerUpType>();
            if(AccelerationFactor > FactorMinLimit) 
            {
                types.Add(PowerUpType.Acceleration);
            }
            if(TorqueFactor > FactorMinLimit) 
            {
                types.Add(PowerUpType.Torque);
            }
            if(ShootFactor > FactorMinLimit) 
            {
                types.Add(PowerUpType.Shoot);
            }

            if(types.Count > 0)
            {
                int index = Random.Range(0, types.Count);
                ModifyFactor(-damageTaken, types[index]);
            }
        }

        ShakeCameraController.Shake();

        if (GetTotalHealth() <= 0f)
        {
            Destroyed();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = collision.relativeVelocity.magnitude;
        float colliderMass = collision.collider.GetComponent<Rigidbody2D>().mass;
        float totalMass = colliderMass + Body.mass;
        // Damage proportional to the collider's mass
        TakeDamage(damage * colliderMass/totalMass);
        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if(damageable != null)
        {
            // Damage proportional to the ship's mass
            damageable.TakeDamage(damage * Body.mass/totalMass);
        }

        ShakeCameraController.Shake();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        PowerUpController powerUp = collider.GetComponent<PowerUpController>();
        if(powerUp)
        {
            ModifyFactor(powerUp.Amount, powerUp.Type);
            powerUp.Consume();
        }
    }

    private void ModifyFactor(float amount, PowerUpType type) 
    {
        switch(type)
        {
            case PowerUpType.Acceleration:
            {
                ModifyAccelerationFactor(amount);
                break;
            }
            case PowerUpType.Torque:
            {
                ModifyTorqueFactor(amount);
                break;
            }
            case PowerUpType.Shoot:
            {
                ModifyShootFactor(amount);
                break;
            }
            case PowerUpType.Shield:
            {
                ModifyShieldFactor(amount);
                break;
            }
        }
    }

    private void ModifyAccelerationFactor(float value) 
    {
        AccelerationFactor = Mathf.Clamp(AccelerationFactor + value, FactorMinLimit, AccelerationFactorMaxLimit);
        UpdateParts(AccelerationParts, AccelerationFactor / AccelerationFactorMaxLimit);
    }

    private void ModifyTorqueFactor(float value) 
    {
        TorqueFactor = Mathf.Clamp(TorqueFactor + value, FactorMinLimit, FactorMaxLimit);
        UpdateParts(RotationParts, TorqueFactor / FactorMaxLimit);
    }

    private void ModifyShootFactor(float value) 
    {
        ShootFactor = Mathf.Clamp(ShootFactor + value, FactorMinLimit, FactorMaxLimit);
        UpdateParts(BlasterParts, ShootFactor / FactorMaxLimit);
    }

    private void ModifyShieldFactor(float value) 
    {
        ShieldFactor = Mathf.Clamp(ShieldFactor + value, FactorMinLimit, FactorMaxLimit);
        UpdateParts(ShieldParts, ShieldFactor / FactorMaxLimit);
    }

    private void UpdateParts(GameObject[] parts, float ratio)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            GameObject part = parts[i];
            bool isActive = (ratio > (float)i/(float)parts.Length);
            if(part.activeSelf != isActive) {
                if(!isActive) {
                    GameObject clone = Instantiate(part);
                    clone.transform.GetChild(0).gameObject.SetActive(true);
                    clone.transform.parent = transform.parent;
                    clone.transform.position = transform.position;
                    Destroy(clone, 2f);
                }
                part.SetActive(isActive);
            }
        }
    }

    private float GetTotalHealth()
    {
        return AccelerationFactor + TorqueFactor + ShootFactor + ShieldFactor;
    }

    private void Destroyed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
