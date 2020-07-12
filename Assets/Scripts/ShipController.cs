using System.Collections.Generic;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ShipController : MonoBehaviour, IDamageable
{
    public float FactorMaxLimit = 2f;
    public float FactorMinLimit = 0f;
    public float HealthLimit = 0f;
    public float ShotCooldownTime = 0.5f;
    public float BaseShootPower = 10f;
    public GameObject Shot;
    public GameObject LookAhead;
    public GameObject AccelerationGaugeObject;
    public GameObject RotationGaugeObject;
    public GameObject ShieldGaugeObject;
    public GameObject BlasterGaugeObject;

    private Rigidbody2D Body;
    private IGauge AccelerationGauge;
    private IGauge RotationGauge;
    private IGauge ShieldGauge;
    private IGauge BlasterGauge;
    private ShakeCameraController ShakeCameraController;

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

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        AccelerationGauge = AccelerationGaugeObject.GetComponent<IGauge>();
        RotationGauge = RotationGaugeObject.GetComponent<IGauge>();
        ShieldGauge = ShieldGaugeObject.GetComponent<IGauge>();
        BlasterGauge = BlasterGaugeObject.GetComponent<IGauge>();
        ShakeCameraController = UnityEngine.Object.FindObjectOfType<CinemachineVirtualCamera>().GetComponent<ShakeCameraController>();
        Body.angularDrag = BaseAngularDrag * AngularDragFactor;
    }

    void Update()
    {
        DesiredRotation = Input.GetAxis("Horizontal");
        DesiredMovement = Mathf.Max(0f, Input.GetAxis("Vertical"));
        Shoot = Input.GetButton("Shoot");
        AccelerationGauge.SetValue(AccelerationFactor/FactorMaxLimit);
        RotationGauge.SetValue(TorqueFactor * BaseTorque / FactorMaxLimit);
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
        } else {
            Body.drag = 0;
        }

        Body.AddTorque(
            -1 * BaseTorque * Time.deltaTime * DesiredRotation * TorqueFactor
        );

        ShotCooldown -= Time.deltaTime;
        if(ShotCooldown <= 0 && Shoot)
        {
            ShotController shot = Instantiate(Shot, LookAhead.transform.position, transform.rotation, transform.parent).GetComponent<ShotController>();
            shot.Fire(direction, BaseShootPower * ShootFactor);
            ShakeCameraController.Shake();

            ShotCooldown = ShotCooldownTime;
        }
    }

    public void TakeDamage(float damageTaken)
    {
        if(ShieldFactor > FactorMinLimit) {
            ModifyShieldFactor(-damageTaken);
        } else {
            
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
        AccelerationFactor = Mathf.Clamp(AccelerationFactor + value, FactorMinLimit, FactorMaxLimit);
    }

    private void ModifyTorqueFactor(float value) 
    {
        TorqueFactor = Mathf.Clamp(TorqueFactor + value, FactorMinLimit, FactorMaxLimit);
    }

    private void ModifyShootFactor(float value) 
    {
        ShootFactor = Mathf.Clamp(ShootFactor + value, FactorMinLimit, FactorMaxLimit);
    }

    private void ModifyShieldFactor(float value) 
    {
        ShieldFactor = Mathf.Clamp(ShieldFactor + value, FactorMinLimit, FactorMaxLimit);
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
