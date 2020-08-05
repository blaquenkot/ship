using UnityEngine;
using DG.Tweening;

public class ArrowController : MonoBehaviour
{
    private const float Padding = 1f;
    private const float MaxPulseCooldown = 0.5f;
    private const float MaxBlinkCooldown = 0.25f;
    public GameObject Target;
    public SpriteRenderer CentralSpriteRenderer;
    public SpriteRenderer ProgressSpriteRenderer;
    
    private Camera Camera;
    private GameObject Ship;
    private SpriteRenderer SpriteRenderer;
    private PointableObject PointableObject;
    private VisibleObject VisibleObject;
    private Color ProgressColor;
    private bool ShouldPulsePeriodically = false;
    private float PulseCooldown = MaxPulseCooldown;
    private float BlinkCooldown = MaxBlinkCooldown;
    private int BlinkTimes = 0;
    private bool CanChangeArrowOpacity = true;
    private Color LowProgressColor;
    private Color MediumProgressColor;
    private Color HighProgressColor;

    void Awake()
    {
        Camera = Camera.main;
        Ship = Object.FindObjectOfType<ShipController>().gameObject;
        SpriteRenderer = GetComponent<SpriteRenderer>();

        ColorUtility.TryParseHtmlString("#932121", out LowProgressColor);
        ColorUtility.TryParseHtmlString("#5a6930", out MediumProgressColor);
        ColorUtility.TryParseHtmlString("#15c141", out HighProgressColor);
        ProgressColor = HighProgressColor;
        ProgressSpriteRenderer.material.SetColor("_Color", ProgressColor);
    }

    void Start()
    {
        PointableObject = Target.GetComponent<PointableObject>();
        VisibleObject = Target.GetComponent<VisibleObject>();
    }

    void FixedUpdate()
    {
        if(!Target || !Ship)
        {
            Destroy(gameObject);
            return;
        }

        if(BlinkTimes > 0)
        {
            CanChangeArrowOpacity = false;
            BlinkCooldown -= Time.deltaTime;
            if(BlinkCooldown <= 0f)
            {
                Color arrowColor = SpriteRenderer.color;
                Color centralSpriteColor = CentralSpriteRenderer.color;
                if(arrowColor.a == 1f) 
                {
                    arrowColor.a = 0f;
                    centralSpriteColor.a = 0f;
                } 
                else 
                {
                    BlinkTimes -= 1;
                    arrowColor.a = 1f;
                    centralSpriteColor.a = 1f;
                }

                DOTween.Sequence()
                    .Join(SpriteRenderer.DOColor(arrowColor, 0.2f))
                    .Join(CentralSpriteRenderer.DOColor(centralSpriteColor, 0.2f))
                    .OnComplete(() => {
                        if(BlinkTimes == 0) {
                            CanChangeArrowOpacity = true;
                        }
                    });
                
                BlinkCooldown = MaxBlinkCooldown;
            }
        }

        if(ShouldPulsePeriodically)
        {
            PulseCooldown -= Time.deltaTime;
            if(PulseCooldown <= 0f)
            {
                Pulse();
                PulseCooldown = MaxPulseCooldown;
            }
        }
        
        Vector3 direction = transform.rotation * Vector2.right;
        Vector3 diffAngleVector = Target.transform.position - transform.position;
        float angleDiff = Vector2.SignedAngle(direction, diffAngleVector);
        transform.rotation = Quaternion.AngleAxis(transform.eulerAngles.z + angleDiff, Vector3.forward);
        
        float maxDistance = (new Vector3(VisibleObject.HalfWidth + Padding, VisibleObject.HalfHeight + Padding, 0f)).magnitude;

        if(BlinkTimes == 0 && !PointableObject.ShowArrowWhileVisible) 
        {
            bool IsNotVisible = !VisibleObject.IsVisible;
            SpriteRenderer.enabled = IsNotVisible;
            CentralSpriteRenderer.enabled = IsNotVisible;
        }

        if(CanChangeArrowOpacity)
        {
            Color arrowColor = SpriteRenderer.color;
            if(arrowColor.a == 1f && VisibleObject.IsVisible || arrowColor.a == 0f && !VisibleObject.IsVisible)
            {
                Color centralSpriteColor = CentralSpriteRenderer.color;
                CanChangeArrowOpacity = false;
                float arcAngle;
                if(VisibleObject.IsVisible)
                {
                    arrowColor.a = 0f;
                    centralSpriteColor.a = 0.8f;
                    arcAngle = 0f;
                }
                else
                {
                    arrowColor.a = 1f;
                    centralSpriteColor.a = 1f;
                    arcAngle = 20f;
                }
                ProgressSpriteRenderer.material.SetFloat("_Arc1", arcAngle);

                DOTween.Sequence()
                        .Join(SpriteRenderer.DOColor(arrowColor, 0.5f))
                        .Join(CentralSpriteRenderer.DOColor(centralSpriteColor, 0.5f))
                        .OnComplete(() => {
                            CanChangeArrowOpacity = true;
                        });
            }
        }

        Vector3 diffVector = Target.transform.position - Ship.transform.position;
        Vector3 fixedPosition = Target.transform.position - diffVector.normalized * maxDistance;
        Vector2 targetInViewportPosition = Camera.WorldToViewportPoint(fixedPosition);
        Vector3 clampedPosition = Camera.ViewportToWorldPoint(new Vector2(Mathf.Clamp(targetInViewportPosition.x, 0.33f, 0.92f), Mathf.Clamp(targetInViewportPosition.y, 0.12f, 0.88f)));
        clampedPosition.z = 0;
        transform.position = clampedPosition;

        float scale = 100f / (100f+diffVector.magnitude) + 0.6f;
        transform.localScale = Vector3.one * Mathf.Clamp(scale, 1f, 1.5f);
    }

    public void SetCentralImage(Sprite Image)
    {
        CentralSpriteRenderer.sprite = Image;
    }

    public void SetProgress(float progress)
    {
        if(ProgressSpriteRenderer)
        {
            UpdateProgressColor(progress);

            float a = VisibleObject.IsVisible ? 360 : 342;
            float b = VisibleObject.IsVisible ? 0 : 27;
            float angle = Mathf.Lerp(a, b, progress);
            ProgressSpriteRenderer.material.SetFloat("_Arc2", angle);
        }
    }

    public void Blink(int times)
    {
        BlinkTimes = times;
    }

    public void TogglePulse()
    {
        ShouldPulsePeriodically = !ShouldPulsePeriodically;
    }

    public void HideArrow()
    {
        if(SpriteRenderer)
        {
            CanChangeArrowOpacity = false;
            Color color = SpriteRenderer.color;
            color.a = 0f;
            SpriteRenderer.DOColor(color, 0.5f);
        }
    }

    void UpdateProgressColor(float progress)
    {
        Color? newColor = null;
        if(progress < 0.2f)
        {
            if(ProgressColor != LowProgressColor)
            {
                TogglePulse();
                newColor = LowProgressColor;
            }
        }
        else if (progress < 0.55f)
        {
            if(ProgressColor != MediumProgressColor)
            {
                newColor = MediumProgressColor;
            }
        }
        else
        {
            if(ProgressColor != HighProgressColor)
            {
                newColor = HighProgressColor;
            }
        }

        if(newColor.HasValue)
        {
            ProgressColor = newColor.Value;
            ProgressSpriteRenderer.material.SetColor("_Color", ProgressColor);
            Pulse();
        }
    }

    void Pulse()
    {
        Vector3 originalScale = transform.localScale;
        transform.DOScale(originalScale * 1.25f, 0.1f).OnComplete(() => {
            transform.DOScale(originalScale, 0.1f);
        }); 
    }
}