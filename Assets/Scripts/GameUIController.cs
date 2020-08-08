using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public Text HPText;
    private MissionController MissionController;
    
    void Awake()
    {
        MissionController = GetComponentInChildren<MissionController>();
    }

    public void MissionSucceed()
    {
        MissionController.MissionSucceed();
    }

    public void MissionFailed()
    {
        MissionController.MissionFailed();
    }

    public void UpdateCurrentTarget(MissionTargetState state)
    {
        MissionController.UpdateCurrentTarget(state);
    }

    public void UpdatePoints(int points)
    {
        MissionController.UpdatePoints(points);
    }

    public void UpdateHealth(float health)
    {
        HPText.text = Mathf.Round(health * 100f).ToString() + "%";
    }
}
