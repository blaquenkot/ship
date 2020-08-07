using UnityEngine;
using Cinemachine;

public class CameraLookableObject : MonoBehaviour
{
    public CinemachineVirtualCamera CinemachineVirtualCamera;

    void OnTriggerEnter2D(Collider2D collider)
    {
        CinemachineVirtualCamera.Priority = 999;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        CinemachineVirtualCamera.Priority = 0;
    }
}