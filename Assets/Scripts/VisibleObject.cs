using UnityEngine;
public class VisibleObject : MonoBehaviour
{
    public bool IsVisible 
    { 
        get 
        {   
            if(!SpriteRenderer.isVisible)
            { 
                return false; 
            }

            Vector3 fixedPosition = new Vector3(transform.position.x + HalfWidth, transform.position.y, transform.position.z);
            return Camera.WorldToViewportPoint(fixedPosition).x > 0.25f;
        } 
    }
    
    public float HalfWidth { get { return SpriteRenderer.bounds.extents.x; } }
    public float HalfHeight { get { return SpriteRenderer.bounds.extents.y; } }

    private SpriteRenderer SpriteRenderer;
    private Camera Camera;

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Camera = Camera.main;
    }
}