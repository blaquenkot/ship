using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour, IDamageable
{
    public float Health = 10f;
    public float ShootPower = 0.5f;
    public float MaxShootCooldown = 1.5f;

    public FlashingLight Light;
    public GameObject Shot;
    public GameObject LookAhead;
    public GameObject Explosion;

    private Rigidbody2D Body;
    private SpriteRenderer SpriteRenderer;
    private ShipController Player;
    private VisibleObject VisibleObject;
    private float MaxRotation = 100;
    private float ShotCooldown = 1.5f;

    void Start() 
    {
        Body = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        VisibleObject = GetComponent<VisibleObject>();
        Player = UnityEngine.Object.FindObjectOfType<ShipController>();
        RotateTowardsPlayer(10f);
        
        transform.DOScale(Vector3.one, 0.75f);
    }

    void FixedUpdate()
    {
        if(VisibleObject.IsVisible && Player) 
        {
            RotateTowardsPlayer(Time.deltaTime);

            transform.position += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0) * Time.deltaTime;

            ShotCooldown -= Time.deltaTime;
            if(ShotCooldown <= 0)
            {
                Vector2 direction = Vector2Utils.Vector2FromAngle(Body.rotation + Random.Range(-15f, 15f));
                ShotController shot = Instantiate(Shot, LookAhead.transform.position, transform.rotation, transform.parent).GetComponent<ShotController>();
                shot.Fire(direction, Body.velocity, ShootPower);

                ShotCooldown = MaxShootCooldown + Random.Range(-0.15f, 0.15f);
            }
        }
    }
    public bool TakeDamage(float damageTaken)
    {
        Health -= damageTaken;

        Light.MakeFlash();

        if(Health <= 0)
        {
            Destroyed();
            return true;
        } 
        else 
        {
            return false;
        }
    }

    public bool IsEnemy()
    {
        return true;
    }

    private void RotateTowardsPlayer(float time)
    {
        var direction = transform.rotation * Vector2.right;
        var diffVector = Player.transform.position - transform.position;
        var angleDiff = Vector2.SignedAngle(direction, diffVector) + Random.Range(-4f, 4f);
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
