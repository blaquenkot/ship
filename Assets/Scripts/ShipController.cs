using UnityEngine.SceneManagement;
using UnityEngine;

public class ShipController : MonoBehaviour, IDamageable
{
    public float ShotCooldownTime = 0.1f;
    public GameObject Shot;
    public GameObject LookAhead;
    public GameObject AccelerationGaugeObject;

    private Rigidbody2D Body;
    private IGauge AccelerationGauge;

    private bool Shoot = false;
    private float DesiredRotation = 0f;
    private float DesiredMovement = 0f;

    private float AccelerationFactor = 1f;
    private float TorqueFactor = 1f;
    private float ShootFactor = 1f;
    private float ShieldFactor = 1f;

    private float ShotCooldown = 0f;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        AccelerationGauge = AccelerationGaugeObject.GetComponent<IGauge>();
    }

    void Update()
    {
        DesiredRotation = Input.GetAxis("Horizontal");
        DesiredMovement = Mathf.Max(0f, Input.GetAxis("Vertical"));
        Shoot = Input.GetButtonUp("Shoot");
        AccelerationGauge.SetValue(AccelerationFactor);
    }

    void FixedUpdate()
    {
        Body.AddForce(DesiredMovement * AccelerationFactor * Time.deltaTime * 500.0f * Vector2FromAngle(Body.rotation));
        Body.AddTorque(-200 * Time.deltaTime * DesiredRotation * TorqueFactor);

        ShotCooldown -= Time.deltaTime;
        if(ShotCooldown <= 0)
        {
            if (Shoot)
            {
                Vector2 lookAheadPosition = LookAhead.transform.position;
                ShotController shot = Instantiate(Shot, LookAhead.transform.position, Quaternion.identity, transform.parent).GetComponent<ShotController>();
                shot.Fire(lookAheadPosition.normalized, ShootFactor);

                ShotCooldown = ShotCooldownTime;
            }
        }
    }

    public void TakeDamage(float damageTaken)
    {
        switch(Random.Range(0, 4))
        {
            case 0: 
            {
                AccelerationFactor -= damageTaken;
                break;
            }
            case 1: 
            {
                TorqueFactor -= damageTaken;
                break;
            }
            case 2: 
            {
                ShootFactor -= damageTaken;
                break;
            }
            case 3: 
            {
                ShieldFactor -= damageTaken;
                break;
            }
        }

        if (TotalHealth() <= 0f)
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
                AccelerationFactor += amount;
                break;
            }
            case PowerUpType.Torque:
            {
                TorqueFactor += amount;
                break;
            }
            case PowerUpType.Shoot:
            {
                ShootFactor += amount;
                break;
            }
            case PowerUpType.Shield:
            {
                ShieldFactor += amount;
                break;
            }
        }
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
    
    private Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    private float TotalHealth()
    {
        return AccelerationFactor + TorqueFactor + ShootFactor + ShieldFactor;
    }

    private void Destroyed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
