using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public GameObject Target;
    public SpriteRenderer PilotSpriteRenderer;

    private Camera Camera;
    private SpriteRenderer SpriteRenderer;
    private SpriteRenderer OrbSpriteRenderer;

    void Awake()
    {
        Camera = Camera.main;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        OrbSpriteRenderer = Target.GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if(!Target)
        {
            Destroy(gameObject);
            return;
        }

        SpriteRenderer.enabled = !OrbSpriteRenderer.isVisible;
        PilotSpriteRenderer.enabled = !OrbSpriteRenderer.isVisible;

        if (SpriteRenderer.enabled)
        {
            var direction = transform.rotation * Vector2.right;
            var diffVector = Target.transform.position - transform.position;
            var angleDiff = Vector2.SignedAngle(direction, diffVector);
            transform.rotation = Quaternion.AngleAxis(transform.eulerAngles.z + angleDiff, Vector3.forward);
            
            Vector2 targetInViewportPosition = Camera.WorldToViewportPoint(Target.transform.position);
            Vector3 clampedPosition = Camera.ViewportToWorldPoint(new Vector2(Mathf.Clamp(targetInViewportPosition.x, 0.31f, 0.94f), Mathf.Clamp(targetInViewportPosition.y, 0.1f, 0.9f)));
            clampedPosition.z = 0;
            transform.position = clampedPosition;

        }
    }
    public void SetPilotImage(Sprite Image)
    {
        PilotSpriteRenderer.sprite = Image;
    }
}