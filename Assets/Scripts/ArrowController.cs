using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public GameObject Target;

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

        if(SpriteRenderer.enabled)
        {
            var direction = transform.rotation * Vector2.right;
            var diffVector = Target.transform.position - transform.position;
            var angleDiff = Vector2.SignedAngle(direction, diffVector);
            transform.rotation = Quaternion.AngleAxis(transform.eulerAngles.z + angleDiff, Vector3.forward);
            
            Vector2 targetInViewportPosition = Camera.WorldToViewportPoint(Target.transform.position);
            Vector3 clampedPosition = Camera.ViewportToWorldPoint(new Vector2(Mathf.Clamp(targetInViewportPosition.x, 0.3f, 0.95f), Mathf.Clamp(targetInViewportPosition.y, 0.05f, 0.95f)));
            clampedPosition.z = 0;
            transform.position = clampedPosition;

            Color color = SpriteRenderer.color;
            color.a = 1/targetInViewportPosition.magnitude;
            SpriteRenderer.color = color;
        }
    }
}