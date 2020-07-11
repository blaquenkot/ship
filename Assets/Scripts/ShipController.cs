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

    private Rigidbody2D Body;
    private IGauge AccelerationGauge;
    private ShakeCameraController ShakeCameraController;

    private bool Shoot = false;
    private float DesiredRotation = 0f;
    private float DesiredMovement = 0f;

    private float ShotCooldown = 0f;
    private float AccelerationFactor = 1f;
    private float TorqueFactor = 1f;
    private float ShootFactor = 1f;
    private float ShieldFactor = 1f;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        AccelerationGauge = AccelerationGaugeObject.GetComponent<IGauge>();
        ShakeCameraController = UnityEngine.Object.FindObjectOfType<CinemachineVirtualCamera>().GetComponent<ShakeCameraController>();
    }

    void Update()
    {
        DesiredRotation = Input.GetAxis("Horizontal");
        DesiredMovement = Mathf.Max(0f, Input.GetAxis("Vertical"));
        Shoot = Input.GetButton("Shoot");
        AccelerationGauge.SetValue(AccelerationFactor/FactorMaxLimit);
    }

    void FixedUpdate()
    {
        Vector2 direction = Vector2FromAngle(Body.rotation);
        Body.AddForce(DesiredMovement * AccelerationFactor * Time.deltaTime * 500.0f * direction);
        Body.AddTorque(-200 * Time.deltaTime * DesiredRotation * TorqueFactor);

        ShotCooldown -= Time.deltaTime;
        if(ShotCooldown <= 0 && Shoot)
        {
            ShotController shot = Instantiate(Shot, LookAhead.transform.position, transform.rotation, transform.parent).GetComponent<ShotController>();
            shot.transform.localScale *= ShootFactor;
            shot.Fire(direction, BaseShootPower * ShootFactor);
            ShakeCameraController.Shake();

            ShotCooldown = ShotCooldownTime;
        }
    }

    public void TakeDamage(float damageTaken)
    {
        switch(Random.Range(0, 4))
        {
            case 0: 
            {
                ModifyAccelerationFactor(-damageTaken);
                break;
            }
            case 1: 
            {
                ModifyTorqueFactor(-damageTaken);
                break;
            }
            case 2: 
            {
                ModifyShootFactor(-damageTaken);
                break;
            }
            case 3: 
            {
                ModifyShieldFactor(-damageTaken);
                break;
            }
        }

        ShakeCameraController.Shake();

        if (GetTotalHealth() <= 0f)
        {
            Destroyed();
        }
    }

    public void AddPowerUp(float amount, PowerUpType type)
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

    void OnCollisionEnter2D(Collision2D collision) 
    {
        // ToDo: Maybe do proportional damage to mass?
        float damage = collision.contacts[0].normalImpulse;
        TakeDamage(damage);
        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        ShakeCameraController.Shake();
    }

    void OnTriggerEnter2D(Collider2D collider) 
    {
        PowerUpController powerUp = collider.GetComponent<PowerUpController>();
        if(powerUp)
        {
            AddPowerUp(powerUp.Amount, powerUp.Type);
            powerUp.Consume();
        }
    }

    private void ModifyAccelerationFactor(float value) {
        AccelerationFactor = Mathf.Clamp(AccelerationFactor + value, FactorMinLimit, FactorMaxLimit); 
    }

    private void ModifyTorqueFactor(float value) {
        TorqueFactor = Mathf.Clamp(TorqueFactor + value, FactorMinLimit, FactorMaxLimit); 
    }

    private void ModifyShootFactor(float value) {
        ShootFactor = Mathf.Clamp(ShootFactor + value, FactorMinLimit, FactorMaxLimit); 
    }

    private void ModifyShieldFactor(float value) {
        ShieldFactor = Mathf.Clamp(ShieldFactor + value, FactorMinLimit, FactorMaxLimit); 
    }
    
    private Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
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
