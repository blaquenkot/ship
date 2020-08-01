using UnityEngine;
using DG.Tweening;

public class PilotController : MonoBehaviour, IDamageable
{
    private const float MaxHealthTimer = 10f;

    public Sprite PilotSprite;
    public FlashingLight Light;
    public WorldController WorldController;
    public ArrowController ArrowController;

    private float AnimationDuration = 1.1f;
    private float ContainerHealth = 10f;
    private float HealthTimer = MaxHealthTimer;
    private SpriteRenderer SpriteRenderer;
    private CircleCollider2D Collider;
    private VisibleObject VisibleObject;

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<CircleCollider2D>();
        VisibleObject = GetComponent<VisibleObject>();
    }

    void Update()
    {
        if(VisibleObject.IsVisible)
        {
            AnimationDuration -= Time.deltaTime;
            if(AnimationDuration <= 0)
            {
                Animate();
                AnimationDuration = 1.1f;
            }
        }

        if(ContainerHealth <= 0f)
        {
            HealthTimer -= Time.deltaTime;
            ArrowController.SetProgress(HealthTimer/MaxHealthTimer);
            if(HealthTimer <= 0)
            {
                HealthTimerExpired();
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

    void HealthTimerExpired()
    {
        // change sprite!
        transform.DOScale(Vector3.one * 0.25f, 0.75f).OnComplete(() => {
            WorldController.PilotDied();
            Destroy(gameObject);
        });
    }

    public bool CanBeConsumed()
    {
        return (ContainerHealth <= 0);
    }

    public void Consume()
    {
        if(CanBeConsumed()) 
        {
            //AudioSource.PlayClipAtPoint(Sound, transform.position);
            Destroy(gameObject);
        }
    }

    public bool ShowArrowWhileVisible()
    {
        return true;
    }

    public bool IsEnemy()
    {
        return false;
    }

    public bool TakeDamage(float damageTaken) 
    {
        Light.MakeFlash();

        ContainerHealth -= damageTaken;

        if(ContainerHealth <= 0)
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