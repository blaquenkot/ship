using UnityEngine;

public class ShotController : MonoBehaviour
{
    public float Speed = 40f;
    private Rigidbody2D Body;
    private SpriteRenderer SpriteRenderer;
    private float HitPower = 0f;

    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Fire(Vector2 direction, float hitPower) 
    {
        HitPower = hitPower;

        Body.velocity = direction * Speed;
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        IDamageable damageable = collision.collider.gameObject.GetComponent<IDamageable>();
        if(damageable != null) 
        {
            damageable.TakeDamage(HitPower);
        }

        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
