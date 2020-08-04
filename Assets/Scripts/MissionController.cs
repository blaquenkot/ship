using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{
    public Text PointsText;

    public void UpdatePoints(int points)
    {
        PointsText.text = "Points: " + points;
    }
}