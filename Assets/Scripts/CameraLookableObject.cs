using UnityEngine;

public class CameraLookableObject : MonoBehaviour
{
    public CameraController CameraController;
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        CameraController.ActivateCamera();
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        CameraController.DeactivateCamera();
    }
}