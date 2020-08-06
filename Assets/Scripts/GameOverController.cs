using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public NewHighscoreController NewHighscoreController;
    public Text PointsLabel;
    public Text TimeLabel;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) 
        {
            OnClickRetry();
        }
    }

    public void UpdateInfo(int points, float time) 
    {
        NewHighscoreController.UpdateInfo(points, time);

        PointsLabel.text = "Points: " + points;
        TimeLabel.text = "Time: " + TimeUtils.GetTimeAsString(time);
    }

    public void OnClickRetry() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}