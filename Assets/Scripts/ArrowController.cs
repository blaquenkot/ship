using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public GameObject Target;
    public SpriteRenderer PilotSpriteRenderer;

    private Camera Camera;
    private SpriteRenderer SpriteRenderer;
    private SpriteRenderer OrbSpriteRenderer;
    private PointableObject PointableObject;
    private VisibleObject VisibleObject;

    void Awake()
    {
        Camera = Camera.main;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        OrbSpriteRenderer = Target.GetComponent<SpriteRenderer>();
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

        var direction = transform.rotation * Vector2.right;
        var diffVector = Target.transform.position - transform.position;
        var angleDiff = Vector2.SignedAngle(direction, diffVector);
        transform.rotation = Quaternion.AngleAxis(transform.eulerAngles.z + angleDiff, Vector3.forward);
        
        bool IsFarFromTarget = diffVector.magnitude > 5f;

        if(!PointableObject.ShowArrowWhileVisible) 
        {
            SpriteRenderer.enabled = IsFarFromTarget;
            PilotSpriteRenderer.enabled = IsFarFromTarget;
        }

        if(IsFarFromTarget || !VisibleObject.IsVisible)
        {
            Vector2 targetInViewportPosition = Camera.WorldToViewportPoint(Target.transform.position);
            Vector3 clampedPosition = Camera.ViewportToWorldPoint(new Vector2(Mathf.Clamp(targetInViewportPosition.x, 0.33f, 0.92f), Mathf.Clamp(targetInViewportPosition.y, 0.12f, 0.88f)));
            clampedPosition.z = 0;
            transform.position = clampedPosition;
        }

        float scale = 100f / (100f+diffVector.magnitude) + 0.6f;
        transform.localScale = Vector3.one * Mathf.Clamp(scale, 1f, 1.5f);
    }

    public void SetPilotImage(Sprite Image)
    {
        PilotSpriteRenderer.sprite = Image;
    }
}