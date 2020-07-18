using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text PointsLabel;
    public Text TimeLabel;

    public void UpdatePoints(int points) 
    {
        PointsLabel.text = "Points: " + points;
    }

    public void UpdateTime(string time) 
    {
        TimeLabel.text = "Time: " + time;
    }
}