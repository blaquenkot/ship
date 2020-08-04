using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text PointsLabel;
    public Image SpecialGauge;

    private float FlickTimer = 0.5f;
    private bool SpecialLoaded = true;
    
    public void UpdatePoints(int points)
    {
        PointsLabel.text = points.ToString();
    }
}
