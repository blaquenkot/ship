using UnityEngine;
using Cinemachine;

public class CameraLookableObject : MonoBehaviour
{
    public CinemachineVirtualCamera CinemachineVirtualCamera;
    
    private float ForceCameraMaxTime = 0f;
    private float ForceCameraTimer = 0f;

    void Update()
    {
        if(ForceCameraMaxTime != 0f)
        {
            ForceCameraTimer += Time.deltaTime;
            if(ForceCameraTimer >= ForceCameraMaxTime)
            {
                DeactivateCamera();
                ForceCameraMaxTime = 0f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        ActivateCamera();
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        DeactivateCamera();
    }

    void ActivateCamera()
    {
        CinemachineVirtualCamera.Priority = 999;
    }

    void DeactivateCamera()
    {
        CinemachineVirtualCamera.Priority = 0;
    }

    public void ForceCamera(float time)
    {
        ActivateCamera();
        ForceCameraMaxTime = time;
    }
}