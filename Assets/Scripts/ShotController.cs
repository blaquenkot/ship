using UnityEngine;

public class ShotController : MonoBehaviour
{
    public WorldController WorldController;
    public float Speed = 40f;
    private Rigidbody2D Body;
    private SpriteRenderer SpriteRenderer;
    private float HitPower = 0f;
    private AudioClip HitSound;

    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        HitSound = Resources.Load<AudioClip>("metal_hit_05");
    }

    public void Fire(Vector2 direction, Vector2 baseVelocity, float hitPower) 
    {
        HitPower = hitPower;

        Body.velocity = baseVelocity + direction * Speed;
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        IDamageable damageable = collision.collider.gameObject.GetComponent<IDamageable>();
        if(damageable != null) 
        {
            bool killed = damageable.TakeDamage(HitPower);
            if(WorldController && killed) 
            {
                WorldController.AddPoints(1);

                if(damageable.IsEnemy()) {
                    WorldController.EnemyKilled(false);
                }
            }

            AudioSource.PlayClipAtPoint(HitSound, transform.position);
        }

        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
