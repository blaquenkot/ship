using UnityEngine;
using DG.Tweening;

public class PilotController : MonoBehaviour, IDamageable
{
    public Sprite PilotSprite;
    public FlashingLight Light;

    private float AnimationDuration = 1.1f;
    private float Health = 10f;
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