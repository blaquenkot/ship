using UnityEngine;

public class InfiniteLayer : MonoBehaviour 
{
    private Camera Camera;
    private SpriteRenderer SpriteRenderer;
    private Vector3 CameraLastPosition = Vector3.zero;

    void Awake() 
    {
        Camera = UnityEngine.Object.FindObjectOfType<Camera>();
        CameraLastPosition = Camera.transform.position;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate() 
    {
        Vector2 diff = (Camera.transform.position - CameraLastPosition);
        SpriteRenderer.size += new Vector2(Mathf.Abs(diff.x), Mathf.Abs(diff.y))*2;
        CameraLastPosition = Camera.transform.position;
    }
}