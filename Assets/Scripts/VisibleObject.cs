using UnityEngine;
public class VisibleObject : MonoBehaviour
{
    public bool IsVisible = false;
    private SpriteRenderer SpriteRenderer;
    private Camera Camera;
    private float HalfWidth;

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Camera = Camera.main;
        HalfWidth = SpriteRenderer.bounds.extents.x;
    }

    void Update()
    {
        if(SpriteRenderer.isVisible)
        {
            Vector3 fixedPosition = new Vector3(transform.position.x + HalfWidth, transform.position.y, transform.position.z);
            IsVisible = Camera.WorldToViewportPoint(fixedPosition).x > 0.25f;
        } 
        else 
        {
            IsVisible = false;
        }
    }
}