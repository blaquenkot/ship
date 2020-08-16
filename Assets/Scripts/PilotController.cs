using UnityEngine;
using DG.Tweening;

public class PilotController : MonoBehaviour, IDamageable
{
    private const float MaxHealthTimer = 10f;

    public SpriteRenderer PilotSpriteRenderer;
    public SpriteRenderer ShipSpriteRenderer;
    public SpriteRenderer RopeSpriteRenderer;
    public SpriteRenderer BugSpriteRenderer;
    public FlashingLight Light;
    public WorldController WorldController;
    public ArrowController ArrowController;

    private float AnimationDuration = 0f;
    private float ContainerHealth = 10f;
    private float HealthTimer = MaxHealthTimer;
    private CircleCollider2D Collider;
    private VisibleObject VisibleObject;
    private bool IsAlive = true;

    void Awake()
    {
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
                    AnimationDuration = 1.2f;
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
        DOTween.Sequence()
                .Append(transform.DOMoveY(transform.position.y + 0.25f, 0.6f).SetEase(Ease.OutQuad))
                .Append(transform.DOMoveY(transform.position.y, 0.5f).SetEase(Ease.InQuad))
                .Append(transform.DOMoveY(transform.position.y - 0.25f, 0.6f).SetEase(Ease.OutQuad))
                .Append(transform.DOMoveY(transform.position.y, 0.5f).SetEase(Ease.InQuad));

        if(ContainerHealth > 0)
        {
            var bugSpriteRendererPosition = BugSpriteRenderer.transform.localPosition;
            DOTween.Sequence()
                    .Append(BugSpriteRenderer.transform.DOLocalMove(bugSpriteRendererPosition * 1.1f, 0.3f))
                    .Append(BugSpriteRenderer.transform.DOLocalMove(bugSpriteRendererPosition, 0.3f))
                    .Append(BugSpriteRenderer.transform.DOLocalMove(bugSpriteRendererPosition * 0.9f, 0.3f))
                    .Append(BugSpriteRenderer.transform.DOLocalMove(bugSpriteRendererPosition, 0.3f));

            var pilotSpriteRendererPositionY = PilotSpriteRenderer.transform.localPosition.y;
            DOTween.Sequence()
                    .Append(RopeSpriteRenderer.transform.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.3f))
                    .Join(PilotSpriteRenderer.transform.DOLocalMoveY(pilotSpriteRendererPositionY - 0.25f, 0.3f))
                    .Append(RopeSpriteRenderer.transform.DOLocalRotate(Vector3.zero, 0.3f))
                    .Join(PilotSpriteRenderer.transform.DOLocalMoveY(pilotSpriteRendererPositionY, 0.3f))
                    .Append(RopeSpriteRenderer.transform.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.3f))
                    .Join(PilotSpriteRenderer.transform.DOLocalMoveY(pilotSpriteRendererPositionY + 0.25f, 0.3f))
                    .Append(RopeSpriteRenderer.transform.DOLocalRotate(Vector3.zero, 0.3f))
                    .Join(PilotSpriteRenderer.transform.DOLocalMoveY(pilotSpriteRendererPositionY, 0.3f));
        }
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
                    WorldController.PilotDied(this);
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
            var shipSpriteRendererColor = ShipSpriteRenderer.color;
            shipSpriteRendererColor.a = 0f;
            var ropeSpriteRendererColor = RopeSpriteRenderer.color;
            ropeSpriteRendererColor.a = 0f;
            var bugSpriteRendererColor = BugSpriteRenderer.color;
            bugSpriteRendererColor.a = 0f;

            DOTween.Sequence()
                    .Join(ShipSpriteRenderer.transform.DOScale(ShipSpriteRenderer.transform.localScale * 0.25f, 0.5f))
                    .Join(ShipSpriteRenderer.DOColor(shipSpriteRendererColor, 0.25f))
                    .Join(RopeSpriteRenderer.transform.DOScale(RopeSpriteRenderer.transform.localScale * 0.25f, 0.5f))
                    .Join(RopeSpriteRenderer.DOColor(ropeSpriteRendererColor, 0.25f))
                    .Join(BugSpriteRenderer.transform.DOScale(BugSpriteRenderer.transform.localScale * 0.25f, 0.5f))
                    .Join(BugSpriteRenderer.DOColor(bugSpriteRendererColor, 0.25f))
                    .Join(PilotSpriteRenderer.transform.DOLocalMove(Vector2.zero, 0.5f))
                    .OnComplete(() => {
                        ShipSpriteRenderer.enabled = false;
                        RopeSpriteRenderer.enabled = false;
                        BugSpriteRenderer.enabled = false;
                    });

            Collider.isTrigger = true;
            return true;
        } 
        else 
        {
            return false;
        }
    }
}