﻿using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class ShipController : MonoBehaviour, IDamageable
{
    private const float MaxSpecialAttackTimer = 15f;
    public float FactorMaxLimit = 8f;
    public float FactorMinLimit = 0f;
    public float HealthLimit = 0f;
    public float ShotCooldownTime = 0.2f;
    public float BaseShootPower = 10f;

    public Material NoLightsMaterial;
    public FlashingLight Light;
    public GameObject Explosion;
    public GameObject SpecialAttack;
    public GaugeController AccelerationGauge;
    public GaugeController RotationGauge;
    public GaugeController ShieldGauge;
    public GaugeController BlasterGauge;
    public SpecialGaugeController SpecialGaugeController;
    public GameObject[] AccelerationParts;
    public GameObject[] RotationParts;
    public GameObject[] ShieldParts;
    public GameObject[] BlasterParts;
    public CannonController[] CannonControllersLevel1;
    public CannonController[] CannonControllersLevel2;
    public CannonController[] CannonControllersLevel3;
    public ParticleSystem TailParticleSystem;
    public ParticleSystem LeftParticleSystem;
    public ParticleSystem RightParticleSystem;
    public WorldController WorldController;

    private Rigidbody2D Body;
    private ShakeCameraController ShakeCameraController;
    private bool Shoot = false;
    private bool ExecuteSpecialAttack = false;
    private float DesiredRotation = 0f;
    private float DesiredMovement = 0f;

    private float SpecialAttackTimer = MaxSpecialAttackTimer;
    private float ShotCooldown = 0f;
    private float AccelerationFactor = 4f;
    private float TorqueFactor = 4f;
    private float ShootFactor = 4f;
    private float ShieldFactor = 4f;
    private float AngularDragFactor = 1f;

    private float MovementDrag = 1f;

    private float BaseAngularDrag = 2f;
    private float BaseAcceleration = 6000f;
    private float BaseTorque = -3000f;
    private float BaseRecoil = -5f;

    private AudioClip ShootSound;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        
        CinemachineVirtualCamera[] cameras= UnityEngine.Object.FindObjectsOfType<CinemachineVirtualCamera>();
        for (int i = 0; i < cameras.Length; i++)
        {
            CinemachineVirtualCamera camera = cameras[i];
            if(camera.name == "CM vcam1")
            {
                ShakeCameraController = camera.GetComponent<ShakeCameraController>();
            }
        }

        ShootSound = Resources.Load<AudioClip>("laser1");
        
        Body.angularDrag = BaseAngularDrag * AngularDragFactor;

        List<CannonController> allCannons = CannonControllersLevel1
                                                .Concat(CannonControllersLevel2)
                                                .Concat(CannonControllersLevel3)
                                                .ToList();
        foreach (var cannon in allCannons)
        {
            cannon.WorldController = WorldController;
        }

        // Forcing update for ship components
        ModifyFactor(0f, PowerUpType.Acceleration);
        ModifyFactor(0f, PowerUpType.Shield);
        ModifyFactor(0f, PowerUpType.Shoot);
        ModifyFactor(0f, PowerUpType.Torque);
        UpdateSpecialAttackTimer(MaxSpecialAttackTimer);
    }

    void Update()
    {
        DesiredRotation = Input.GetAxis("Horizontal");
        DesiredMovement = Mathf.Max(0f, Input.GetAxis("Vertical"));
        Shoot = Input.GetButton("Shoot");
        ExecuteSpecialAttack = Input.GetKey(KeyCode.X);
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

        UpdateSpecialAttackTimer(SpecialAttackTimer + Time.deltaTime);
       
        if (SpecialAttackTimer >= MaxSpecialAttackTimer)
        {
            if (ExecuteSpecialAttack)
            {
                SpecialAttackController specialAttackController = Instantiate(SpecialAttack, transform.position, transform.rotation, transform.parent).GetComponent<SpecialAttackController>();
            
                specialAttackController.Fire(gameObject, WorldController);
            
                ShakeCameraController.Shake(SpecialAttackController.Duration);
                WorldController.Flash(2);
                UpdateSpecialAttackTimer(0f);
            }
        }
    }

    public bool TakeDamage(float damageTaken)
    {
        Light.MakeFlash();

        if(ShieldFactor > FactorMinLimit) 
        {
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

                if(damageable.IsEnemy()) 
                {
                    WorldController.EnemyKilled(false);
                }
            }
        }

        ShakeCameraController.Shake();
    }

    void UpdateSpecialAttackTimer(float newValue)
    {
        SpecialAttackTimer = newValue;
        SpecialGaugeController.SetValue(Mathf.Clamp01(SpecialAttackTimer/MaxSpecialAttackTimer));
    }

    public void EnemyKilled(bool wasSpecialAttack)
    {
        if(!wasSpecialAttack)
        {
            UpdateSpecialAttackTimer(SpecialAttackTimer + 2.5f);

            PowerUpType weakerType = new Dictionary<PowerUpType, float>() {
                { PowerUpType.Acceleration, AccelerationFactor },
                { PowerUpType.Shield, ShieldFactor },
                { PowerUpType.Shoot, ShootFactor },
                { PowerUpType.Torque, TorqueFactor },
            }.OrderBy(kvp => kvp.Value).First().Key;

            ModifyFactor(0.5f, weakerType);
        }
    }

    public bool IsEnemy()
    {
        return false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        PickupableObject pickupableObject = collider.GetComponent<PickupableObject>();
        if(pickupableObject && pickupableObject.CanBePickedUp())
        {
            pickupableObject.PickUp(gameObject);

            PowerUpController powerUp = collider.GetComponent<PowerUpController>();
            if(powerUp)
            {
                ModifyFactor(powerUp.Amount, powerUp.Type);
            } 
            else 
            {
                PilotController Pilot = collider.GetComponent<PilotController>();
                if(Pilot) 
                {
                    UpdateSpecialAttackTimer(MaxSpecialAttackTimer);
                    WorldController.PilotPickedUp();
                } 
            }
        } 
        else 
        {
            SpaceStationController SpaceStation = collider.GetComponent<SpaceStationController>();
            if(SpaceStation)
            {
                WorldController.SpaceStationReached();
                transform.DOScale(transform.localScale * 0.5f, 0.25f).OnComplete(() => {
                    Destroy(gameObject);
                });
            }
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
        float ratio = AccelerationFactor / FactorMaxLimit;
        UpdateParts(AccelerationParts, ratio);
        AccelerationGauge.SetValue(ratio);

        ParticleSystem.MainModule main = TailParticleSystem.main;
        TailParticleSystem.Pause();
        main.startSpeed = new ParticleSystem.MinMaxCurve(Mathf.Lerp(2.5f, 10f, ratio));
        main.startLifetime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(0.05f, 0.1f, ratio));
        main.duration = Mathf.Lerp(1f, 4f, ratio);
        TailParticleSystem.Play();
    }

    private void ModifyTorqueFactor(float value) 
    {
        TorqueFactor = Mathf.Clamp(TorqueFactor + value, FactorMinLimit, FactorMaxLimit);
        float ratio = TorqueFactor / FactorMaxLimit;
        UpdateParts(RotationParts, ratio);
        RotationGauge.SetValue(ratio);

        ParticleSystem.MinMaxCurve startSpeedCurve = new ParticleSystem.MinMaxCurve(Mathf.Lerp(1f, 6f, ratio));
        ParticleSystem.MinMaxCurve startLifetimeCurve = new ParticleSystem.MinMaxCurve(Mathf.Lerp(0.05f, 0.1f, ratio));
        float duration = Mathf.Lerp(0.5f, 1f, ratio);

        LeftParticleSystem.Pause();
        ParticleSystem.MainModule leftMain = LeftParticleSystem.main;
        leftMain.startSpeed = startSpeedCurve;
        leftMain.startLifetime = startLifetimeCurve;
        leftMain.duration = duration;
        LeftParticleSystem.Play();

        RightParticleSystem.Pause();
        ParticleSystem.MainModule rightMain = RightParticleSystem.main;
        rightMain.startSpeed = startSpeedCurve;
        rightMain.startLifetime = startLifetimeCurve;
        rightMain.duration = duration;
        RightParticleSystem.Play();
    }

    private void ModifyShootFactor(float value) 
    {
        ShootFactor = Mathf.Clamp(ShootFactor + value, FactorMinLimit, FactorMaxLimit);
        UpdateParts(BlasterParts, ShootFactor / FactorMaxLimit);
        BlasterGauge.SetValue(ShootFactor / FactorMaxLimit);
    }

    private void ModifyShieldFactor(float value) 
    {
        ShieldFactor = Mathf.Clamp(ShieldFactor + value, FactorMinLimit, FactorMaxLimit);
        UpdateParts(ShieldParts, ShieldFactor / FactorMaxLimit);
        ShieldGauge.SetValue(ShieldFactor / FactorMaxLimit);
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
                    clone.transform.parent = transform.parent;
                    clone.transform.position = transform.position;
                    clone.transform.localScale = transform.localScale;
                    SpriteRenderer renderer = clone.GetComponent<SpriteRenderer>();
                    renderer.material = NoLightsMaterial;
                    renderer.sortingOrder = -100;
                    clone.transform
                            .DOScale(Vector3.one * 0.1f, 4f)
                            .OnComplete(() => {
                                Destroy(clone);
                            });
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

        GameObject explosion = Instantiate(Explosion);
        explosion.transform.parent = transform.parent;
        explosion.transform.position = transform.position;

        Destroy(gameObject);
    }
}
