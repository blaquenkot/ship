using UnityEngine;
using UnityEngine.UI;

public class YouWonController : MonoBehaviour
{
    public NewHighscoreController NewHighscoreController;
    public Text PointsLabel;
    public Text TimeLabel;

    public void UpdateInfo(int points, float time) 
    {
        NewHighscoreController.UpdateInfo(points, time);

        PointsLabel.text = "Points: " + points;
        TimeLabel.text = "Time: " + TimeUtils.GetTimeAsString(time);
    }
}