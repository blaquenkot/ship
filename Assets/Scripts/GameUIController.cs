using UnityEngine;

public class GameUIController : MonoBehaviour
{
    private MissionController MissionController;
    
    void Awake()
    {
        MissionController = GetComponentInChildren<MissionController>();
    }

    public void UpdateCurrentTarget(MissionTargetState state)
    {
        MissionController.UpdateCurrentTarget(state);
    }

    public void UpdatePoints(int points)
    {
        MissionController.UpdatePoints(points);
    }
}
