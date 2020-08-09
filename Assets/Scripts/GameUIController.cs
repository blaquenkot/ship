using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUIController : MonoBehaviour
{
    private const float MaxBlinkCooldown = 0.25f;

    public Text HPText;
    public Text InfoText;

    private MissionController MissionController;
    private int BlinkTextTimes = 0;
    private float BlinkTextCooldown = 0f;
    
    void Awake()
    {
        MissionController = GetComponentInChildren<MissionController>();
    }

    void Update()
    {
        if(BlinkTextTimes > 0)
        {
            BlinkTextCooldown -= Time.deltaTime;
            if(BlinkTextCooldown <= 0f)
            {
                Color color = InfoText.color;
                if(color.a == 1f) 
                {
                    color.a = 0f;
                    BlinkTextTimes -= 1;
                } 
                else 
                {
                    color.a = 1f;
                }

                InfoText.DOColor(color, 0.2f);
                
                BlinkTextCooldown = MaxBlinkCooldown;
            }
        }
    }

    public void BlinkText(int times)
    {
        BlinkTextTimes = times;
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