using UnityEngine;

public class ParallaxLayer : MonoBehaviour 
{
    public float MovementFactor = 0.025f;
    private ShipController Ship;
    private Vector3 ShipLastPosition = Vector3.zero;

    void Awake() 
    {
        Ship = UnityEngine.Object.FindObjectOfType<ShipController>();
        ShipLastPosition = Ship.transform.position;
    }

    void FixedUpdate() 
    {
        Vector3 diff = Ship.transform.position - ShipLastPosition;
        transform.position += diff * MovementFactor;
        ShipLastPosition = Ship.transform.position;
    }
}