using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using DG.Tweening;

public class ShotController : MonoBehaviour
{
    private const float DestructionDuration = 1f;

    public WorldController WorldController;
    public float Speed = 40f;
    private Rigidbody2D Rigidbody;
    private SpriteRenderer SpriteRenderer;
    private SpriteRenderer[] ExplosionsSpriteRenderers;
    private Light2D Light;
    private float HitPower = 0f;
    private AudioClip HitSound;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        ExplosionsSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Light = GetComponentInChildren<Light2D>();
        HitSound = Resources.Load<AudioClip>("metal_hit_05");
    }

    public void Fire(Vector2 direction, Vector2 baseVelocity, float hitPower) 
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        HitPower = hitPower;

        Rigidbody.velocity = baseVelocity + direction * Speed;
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


        Destroyed(true);
    }

    void OnBecameInvisible()
    {
        Destroyed(false);
    }

    void Destroyed(bool animated)
    {
        if(!animated) {
            Destroy(gameObject);
            return;
        }
        
        Rigidbody.velocity = Vector2.zero;
        SpriteRenderer.enabled = false;
        Light.enabled = false;
        Sequence sequence = DOTween.Sequence();
        foreach (var explosion in ExplosionsSpriteRenderers)
        {   
            explosion.enabled = true;
            Transform explosionTransform = explosion.transform;
            Vector3 newScale = explosionTransform.localScale * 0.25f;
            Vector3 newPosition = explosionTransform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
            Color newColor = explosion.color;
            newColor.a = 0f;
            sequence.Join(explosion.DOColor(newColor, DestructionDuration));
            sequence.Join(explosionTransform.DOScale(newScale, DestructionDuration));
            sequence.Join(explosionTransform.DOMove(newPosition, DestructionDuration));
        }
        sequence.Play().OnComplete(() => {
            Destroy(gameObject);
        });
    }
}
