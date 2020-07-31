using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text PointsLabel;
    public Image SpecialGauge;
    public Sprite SpecialOnSprite;
    public Sprite SpecialOffSprite;
    private float FlickTimer = 0.5f;
    private bool SpecialLoaded = true;
    
    void Update()
    {
        if (SpecialLoaded)
        {
            FlickTimer -= Time.deltaTime;
            if (FlickTimer <= 0f)
            {
                if (SpecialGauge.sprite == SpecialOnSprite)
                {
                    SpecialGauge.sprite = SpecialOffSprite;
                    FlickTimer = 0.2f;
                }
                else
                {
                    SpecialGauge.sprite = SpecialOnSprite;
                    FlickTimer = 0.7f;
                }

            }
        }
    }
    public void UpdatePoints(int points)
    {
        PointsLabel.text = points.ToString();
    }
    public void UpdateSpecialGauge(bool isLoaded)
    {
        if (SpecialLoaded == isLoaded)
        {
            return;
        }
        SpecialLoaded = isLoaded;

        if (isLoaded)
        {
            SpecialGauge.sprite = SpecialOnSprite;

        }
        else
        {
            SpecialGauge.sprite = SpecialOffSprite;
        }
    }
}
