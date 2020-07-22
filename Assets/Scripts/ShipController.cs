using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ShipController : MonoBehaviour, IDamageable
{
    public float FactorMaxLimit = 8f;
    public float FactorMinLimit = 0f;
    public float HealthLimit = 0f;
    public float ShotCooldownTime = 0.2f;
    public float BaseShootPower = 10f;
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
    public GameObject LeftParticleObject;
    public GameObject RightParticleObject;
    public GameObject WorldControllerObject;

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
    private ParticleSystem LeftParticleSystem;
    private ParticleSystem RightParticleSystem;
    private WorldController WorldController;
    private bool Shoot = false;
    private float DesiredRotation = 0f;
    private float DesiredMovement = 0f;

    private float ShotCooldown = 0f;
    private float AccelerationFactor = 4f;
    private float TorqueFactor = 4f;
    private float ShootFactor = 4f;
    private float ShieldFactor = 4f;
    private float AngularDragFactor = 1f;

    private float MovementDrag = 1f;

    private float BaseAngularDrag = 2f;
    private float BaseAcceleration = 11000f;
    private float BaseTorque = -3000f;
    private float BaseRecoil = -5f;

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
        LeftParticleSystem = LeftParticleObject.GetComponent<ParticleSystem>();
        RightParticleSystem = RightParticleObject.GetComponent<ParticleSystem>();
        WorldController = WorldControllerObject.GetComponent<WorldController>();

        ShootSound = Resources.Load<AudioClip>("laser1");
        
        Body.angularDrag = BaseAngularDrag * AngularDragFactor;

        CannonControllersLevel1 = new CannonController[CannonsLevel1.Length];
        CannonControllersLevel2 = new CannonController[CannonsLevel2.Length];
        CannonControllersLevel3 = new CannonController[CannonsLevel3.Length];

        for (int i = 0; i < CannonsLevel1.Length; i++)
        {
            CannonControllersLevel1[i] = CannonsLevel1[i].GetComponent<CannonController>();
            CannonControllersLevel1[i].WorldController = WorldController;
        }
        for (int i = 0; i < CannonsLevel2.Length; i++)
        {
            CannonControllersLevel2[i] = CannonsLevel2[i].GetComponent<CannonController>();
            CannonControllersLevel2[i].WorldController = WorldController;
        }
        for (int i = 0; i < CannonsLevel3.Length; i++)
        {
            CannonControllersLevel3[i] = CannonsLevel3[i].GetComponent<CannonController>();
            CannonControllersLevel3[i].WorldController = WorldController;
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
        AccelerationGauge.SetValue(AccelerationFactor / FactorMaxLimit);
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
                    BaseAcceleration *
                    direction
            );

            if(AccelerationFactor > 0 && TailParticleSystem.isStopped) 
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
            BaseTorque * Time.deltaTime * DesiredRotation * TorqueFactor
        );

        if(DesiredRotation != 0 && TorqueFactor > 0) 
        {
            if(DesiredRotation > 0) 
            {
                if(LeftParticleSystem.isStopped) 
                {
                    LeftParticleSystem.Play();
                }
                if(RightParticleSystem.isPlaying) 
                {
                    RightParticleSystem.Stop();
                }
            } 
            else 
            {
                if(RightParticleSystem.isStopped) 
                {
                    RightParticleSystem.Play();
                }
                if(LeftParticleSystem.isPlaying) 
                {
                    LeftParticleSystem.Stop();
                }
            }
        } 
        else 
        {
            if(LeftParticleSystem.isPlaying) 
            {
                LeftParticleSystem.Stop();
            }
            if(RightParticleSystem.isPlaying)
            {
                RightParticleSystem.Stop();
            }
        }

        ShotCooldown -= Time.deltaTime;
        if(ShotCooldown <= 0 && Shoot && ShootFactor > 0f)
        {
            if(BlasterParts[0].activeSelf) 
            {
                foreach (var cannon in CannonControllersLevel1)
                {
                    cannon.Fire(direction, Body.velocity);
                }
            }
            if(BlasterParts[1].activeSelf) 
            {
                foreach (var cannon in CannonControllersLevel2)
                {
                    cannon.Fire(direction, Body.velocity);
                }
            }
            if(BlasterParts[2].activeSelf) 
            {
                foreach (var cannon in CannonControllersLevel3)
                {
                    cannon.Fire(direction, Body.velocity);
                }
            }
            
            AudioSource.PlayClipAtPoint(ShootSound, transform.position);

            Body.AddForce(BaseRecoil * Mathf.Exp(ShootFactor) * direction);

            ShakeCameraController.Shake();

            ShotCooldown = ShotCooldownTime;
        }
    }

    public bool TakeDamage(float damageTaken)
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
            StartCoroutine("Destroyed");
            return true;
        }
        else 
        {
            return false;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = Mathf.Sqrt(collision.relativeVelocity.magnitude);
        float colliderMass = collision.collider.GetComponent<Rigidbody2D>().mass;
        float totalMass = colliderMass + Body.mass;

        // Damage proportional to the collider's mass
        TakeDamage(damage * colliderMass/totalMass);
        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if(damageable != null)
        {
            // Damage proportional to the ship's mass
            bool killed = damageable.TakeDamage(damage * Body.mass/totalMass);
            if(killed) 
            {
                WorldController.AddPoints(1);
            }
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
        Body.mass += amount * 0.5f;

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
        AccelerationFactor = Mathf.Clamp(AccelerationFactor + value, FactorMinLimit, FactorMaxLimit);
        UpdateParts(AccelerationParts, AccelerationFactor / FactorMaxLimit);
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
        WorldController.ShipDestroyed();
    }
}
