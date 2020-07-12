﻿using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public float Health = 10f;
    public float ShootPower = 0.5f;
    public GameObject Shot;
    public GameObject LookAhead;
    public GameObject Explosion;

    private Rigidbody2D Body;
    private ShipController Player;

    private float MaxRotation = 100;
    private float ShotCooldown = 1.5f;
    private bool IsVisible = true;

    void Start() 
    {
        Body = GetComponent<Rigidbody2D>();
        Player = UnityEngine.Object.FindObjectOfType<ShipController>();
        RotateTowardsPlayer(10f);
    }

    void FixedUpdate()
    {
        if(IsVisible) 
        {
            RotateTowardsPlayer(Time.deltaTime);

            transform.position += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0) * Time.deltaTime;

            ShotCooldown -= Time.deltaTime;
            if(ShotCooldown <= 0)
            {
                Vector2 direction = Vector2Utils.Vector2FromAngle(Body.rotation);
                ShotController shot = Instantiate(Shot, LookAhead.transform.position, transform.rotation, transform.parent).GetComponent<ShotController>();
                shot.Fire(direction, ShootPower);

                ShotCooldown = 1.5f;
            }
        }
    }

    void OnBecameVisible()
    {
        IsVisible = true;
    }

    void OnBecameInvisible()
    {
        IsVisible = false;
    }

    public void TakeDamage(float damageTaken)
    {
        Health -= damageTaken;

        if(Health <= 0)
        {
            Destroyed();
        }
    }

    private void RotateTowardsPlayer(float time)
    {
        var direction = transform.rotation * Vector2.right;
        var diffVector = Player.transform.position - transform.position;
        var angleDiff = Vector2.SignedAngle(direction, diffVector);
        var clampedDiff = Mathf.Clamp(
            angleDiff,
            -MaxRotation * time,
            MaxRotation * time
        );

        transform.rotation = Quaternion.AngleAxis(
            transform.eulerAngles.z + clampedDiff,
            Vector3.forward
        );
    }

    private void Destroyed()
    {
        GameObject explosion = Instantiate(Explosion);
        explosion.transform.parent = transform.parent;
        explosion.transform.position = transform.position;
        Destroy(gameObject);
    }
}
