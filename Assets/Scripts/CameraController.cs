using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera CinemachineVirtualCamera;
    private float DefaultCameraOrthographicSize = 0f;    
    private float DesiredCameraOrthographicSize = 0f;
    private float AdjustZoomMaxTime = 0.5f;
    private float AdjustZoomTimer = 0f;
    private float ForceCameraMaxTime = 0f;
    private float ForceCameraTimer = 0f;
    private bool ForcedCamera = false;

    void Awake()
    {
        CinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if(AdjustZoomTimer != 0f)
        {
            AdjustZoomTimer += Time.deltaTime;
            CinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(CinemachineVirtualCamera.m_Lens.OrthographicSize, DesiredCameraOrthographicSize, AdjustZoomTimer);
            if(AdjustZoomTimer >= AdjustZoomMaxTime)
            {
                CinemachineVirtualCamera.m_Lens.OrthographicSize = DesiredCameraOrthographicSize;
                AdjustZoomTimer = 0f;
            }
        }
        else if(ForceCameraMaxTime != 0f)
        {
            ForceCameraTimer += Time.deltaTime;
            CinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(CinemachineVirtualCamera.m_Lens.OrthographicSize, DefaultCameraOrthographicSize, ForceCameraTimer);
            if(ForceCameraTimer >= ForceCameraMaxTime)
            {
                CinemachineVirtualCamera.m_Lens.OrthographicSize = DefaultCameraOrthographicSize;
                ForcedCamera = false;
                DeactivateCamera();
                ForceCameraMaxTime = 0f;
            }
        }
    }

    public void ActivateCamera()
    {
        CinemachineVirtualCamera.Priority = 999;
    }

    public void DeactivateCamera()
    {
        if(!ForcedCamera)
        {
            CinemachineVirtualCamera.Priority = 0;
        }
    }

    public void SetOrthographicSize(float size)
    {
        if(!ForcedCamera)
        {
            AdjustZoomTimer = 0.001f;
            DesiredCameraOrthographicSize = size;
        }
    }

    public void ForceCamera(float adjustTime, float duration, float orthographicSize)
    {
        ForcedCamera = true;
        ForceCameraMaxTime = duration;
        AdjustZoomMaxTime = adjustTime;
        DefaultCameraOrthographicSize = CinemachineVirtualCamera.m_Lens.OrthographicSize;
        DesiredCameraOrthographicSize = orthographicSize;
        ActivateCamera();
    }
}