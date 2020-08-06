using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public NewHighscoreController NewHighscoreController;
    public Text PointsLabel;
    public Text TimeLabel;

    private int Points;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) 
        {
            OnClickRetry();
        }
    }

    public void UpdateInfo(int points, string time) 
    {
        Points = points;

        PointsLabel.text = "Points: " + Points;
        TimeLabel.text = "Time: " + time;
    }

    public void OnClickSaveHighscore() 
    {
        NewHighscoreController.Show(Points);
    }
    
    public void OnClickRetry() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}