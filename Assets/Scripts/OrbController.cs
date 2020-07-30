using UnityEngine;
using DG.Tweening;

public class OrbController : MonoBehaviour, IDamageable
{
    public Sprite PilotSprite;
    public FlashingLight Light;
    public bool IsVisible = false;

    private float AnimationDuration = 1.1f;
    private float Health = 10f;
    private SpriteRenderer SpriteRenderer;
    private CircleCollider2D Collider;

    private Camera Camera;
    private float HalfWidth;

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<CircleCollider2D>();
        Camera = Camera.main;
        HalfWidth = SpriteRenderer.bounds.extents.x;
    }

    void Start()
    {
        InvokeRepeating("Animate", 0, 1.1f);
    }

    void Update()
    {
        if(SpriteRenderer.isVisible)
        {
            Vector3 fixedPosition = new Vector3(transform.position.x + HalfWidth, transform.position.y, transform.position.z);
            IsVisible = Camera.WorldToViewportPoint(fixedPosition).x > 0.25f;
        } else {
            IsVisible = false;
        }

        if(IsVisible)
        {
            AnimationDuration -= Time.deltaTime;
            if(AnimationDuration <= 0)
            {
                Animate();
                AnimationDuration = 1.1f;
            }
        }
    }

    void Animate()
    {
        transform
				.DOMoveY(transform.position.y + 0.5f, 0.5f)
				.SetEase(Ease.OutQuad)
				.OnComplete(() =>
				{	
					transform
						.DOMoveY(transform.position.y - 0.5f, 0.5f)
						.SetEase(Ease.InQuad);
				});
    }

    public bool CanBeConsumed()
    {
        return (Health <= 0);
    }

    public void Consume()
    {
        if(CanBeConsumed()) {
            //AudioSource.PlayClipAtPoint(Sound, transform.position);
            Destroy(gameObject);
        }
    }

    public bool IsEnemy()
    {
        return false;
    }

    public bool TakeDamage(float damageTaken) 
    {
        Light.MakeFlash();

        Health -= damageTaken;

        if(Health <= 0)
        {
            SpriteRenderer.sprite = PilotSprite;
            Collider.isTrigger = true;
            return true;
        } 
        else 
        {
            return false;
        }
    }
}