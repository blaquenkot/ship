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
    private bool IsAlive = true;

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<CircleCollider2D>();
        VisibleObject = GetComponent<VisibleObject>();
    }

    void Update()
    {
        if(IsAlive)
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
        // change sprite?
        IsAlive = false;
        ArrowController.HideArrow();
        ArrowController.TogglePulse();
        DOTween.Sequence()
                .Join(transform.DOScale(Vector3.one * 0.25f, 0.75f))
                .Join(ArrowController.gameObject.transform.DOScale(Vector3.one * 0.25f, 0.75f))
                .Play()
                .OnComplete(() => {
                    WorldController.PilotDied();
                    Destroy(gameObject);
                });
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
        if(!VisibleObject.IsVisible) 
        {
            return false;
        }

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