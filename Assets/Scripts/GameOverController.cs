using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Text PointsLabel;
    public Text TimeLabel;

    public void UpdateInfo(int points, string time) 
    {
        PointsLabel.text = "Points: " + points;
        TimeLabel.text = "Time: " + time;
    }

    public void OnClickRetry() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}