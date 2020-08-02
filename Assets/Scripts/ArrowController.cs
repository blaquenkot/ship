using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public GameObject Target;
    public SpriteRenderer CentralSpriteRenderer;
    public SpriteRenderer ProgressSpriteRenderer;
    
    private Camera Camera;
    private SpriteRenderer SpriteRenderer;
    private PointableObject PointableObject;
    private VisibleObject VisibleObject;
    private float HideAndShowCooldown = 0.2f;
    private int HideAndShowTimes = 0;

    void Awake()
    {
        Camera = Camera.main;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        PointableObject = Target.GetComponent<PointableObject>();
        VisibleObject = Target.GetComponent<VisibleObject>();
    }

    void FixedUpdate()
    {
        if(!Target)
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
        
        var direction = transform.rotation * Vector2.right;
        var diffVector = Target.transform.position - transform.position;
        var angleDiff = Vector2.SignedAngle(direction, diffVector);
        transform.rotation = Quaternion.AngleAxis(transform.eulerAngles.z + angleDiff, Vector3.forward);
        
        bool IsFarFromTarget = diffVector.magnitude > 5f;
        bool IsNotVisible = !VisibleObject.IsVisible;
        if(!PointableObject.ShowArrowWhileVisible) 
        {
            SpriteRenderer.enabled = IsNotVisible;
            CentralSpriteRenderer.enabled = IsNotVisible;
        }

        if(IsFarFromTarget || IsNotVisible)
        {
            Vector2 targetInViewportPosition = Camera.WorldToViewportPoint(Target.transform.position);
            Vector3 clampedPosition = Camera.ViewportToWorldPoint(new Vector2(Mathf.Clamp(targetInViewportPosition.x, 0.33f, 0.92f), Mathf.Clamp(targetInViewportPosition.y, 0.12f, 0.88f)));
            clampedPosition.z = 0;
            transform.position = clampedPosition;
        }

        float scale = 100f / (100f+diffVector.magnitude) + 0.6f;
        transform.localScale = Vector3.one * Mathf.Clamp(scale, 1f, 1.5f);
    }

    public void SetCentralImage(Sprite Image)
    {
        CentralSpriteRenderer.sprite = Image;
    }

    public void SetProgress(float progress)
    {
        float angle = Mathf.Lerp(342, 27, progress);
        ProgressSpriteRenderer.material.SetFloat("_Arc2", angle);
    }

    public void HideAndShow(int times)
    {
        HideAndShowCooldown = 0.2f;
        HideAndShowTimes = times;
    }
}