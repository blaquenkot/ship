using UnityEngine;
using DG.Tweening;

public class ArrowController : MonoBehaviour
{
    private const float Padding = 1f;
    public GameObject Target;
    public SpriteRenderer CentralSpriteRenderer;
    public SpriteRenderer ProgressSpriteRenderer;
    
    private Camera Camera;
    private GameObject Ship;
    private SpriteRenderer SpriteRenderer;
    private PointableObject PointableObject;
    private VisibleObject VisibleObject;
    private Color ProgressColor;
    private float HideAndShowCooldown = 0.2f;
    private int HideAndShowTimes = 0;
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
        ColorUtility.TryParseHtmlString("#7f3470", out MediumProgressColor);
        ColorUtility.TryParseHtmlString("#5c54f9", out HighProgressColor);
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

        if(HideAndShowTimes > 0)
        {
            HideAndShowCooldown -= Time.deltaTime;
            if(HideAndShowCooldown <= 0f)
            {
                if(!SpriteRenderer.enabled) 
                {
                    SpriteRenderer.enabled = true;
                    CentralSpriteRenderer.enabled = true;
                    HideAndShowTimes -= 1;
                } 
                else 
                {
                    SpriteRenderer.enabled = false;
                    CentralSpriteRenderer.enabled = false;
                }
                
                HideAndShowCooldown = 0.2f;
            }
        }
        
        Vector3 direction = transform.rotation * Vector2.right;
        Vector3 diffAngleVector = Target.transform.position - transform.position;
        float angleDiff = Vector2.SignedAngle(direction, diffAngleVector);
        transform.rotation = Quaternion.AngleAxis(transform.eulerAngles.z + angleDiff, Vector3.forward);
        
        float maxDistance = (new Vector3(VisibleObject.HalfWidth + Padding, VisibleObject.HalfHeight + Padding, 0f)).magnitude;

        if(HideAndShowTimes == 0 && !PointableObject.ShowArrowWhileVisible) 
        {
            bool IsNotVisible = !VisibleObject.IsVisible;
            SpriteRenderer.enabled = IsNotVisible;
            CentralSpriteRenderer.enabled = IsNotVisible;
        }

        if(CanChangeArrowOpacity)
        {
            Color color = SpriteRenderer.color;
            if(color.a == 1f && VisibleObject.IsVisible || color.a == 0f && !VisibleObject.IsVisible)
            {
                CanChangeArrowOpacity = false;
                float arcAngle;
                if(VisibleObject.IsVisible)
                {
                    color.a = 0f;
                    arcAngle = 0f;

                }
                else
                {
                    color.a = 1f;
                    arcAngle = 20f;
                }
                color.a = VisibleObject.IsVisible ? 0f : 1f;
                ProgressSpriteRenderer.material.SetFloat("_Arc1", arcAngle);

                SpriteRenderer.DOColor(color, 0.5f).OnComplete(() => {
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
        if(!ProgressSpriteRenderer)
        {
            return;
        }

        UpdateProgressColor(progress);

        float a = VisibleObject.IsVisible ? 360 : 342;
        float b = VisibleObject.IsVisible ? 0 : 27;
        float angle = Mathf.Lerp(a, b, progress);
        ProgressSpriteRenderer.material.SetFloat("_Arc2", angle);
    }

    public void HideAndShow(int times)
    {
        HideAndShowCooldown = 0.2f;
        HideAndShowTimes = times;
    }

    public void HideArrow()
    {
        CanChangeArrowOpacity = false;
        Color color = SpriteRenderer.color;
        color.a = 0f;
        SpriteRenderer.DOColor(color, 0.5f);
    }

    void UpdateProgressColor(float progress)
    {
        Color? newColor = null;
        if(progress < 0.2f)
        {
            if(ProgressColor != LowProgressColor)
            {
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