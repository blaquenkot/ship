using Cinemachine;
public class CinemachineVirtualCameraUtils
{
    static public CinemachineVirtualCamera GetHigherPriorityCamera()
    {
        CinemachineVirtualCamera[] cameras = UnityEngine.Object.FindObjectsOfType<CinemachineVirtualCamera>();
        CinemachineVirtualCamera higherPriorityCamera = cameras[0];
        for (int i = 1; i < cameras.Length; i++)
        {
            CinemachineVirtualCamera camera = cameras[i];
            if(camera.Priority > higherPriorityCamera.Priority)
            {
                higherPriorityCamera = camera;
            }
        }

        return higherPriorityCamera;
    }
}
