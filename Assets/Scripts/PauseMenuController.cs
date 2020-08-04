using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public Text MuteButtonText;
    private AudioController AudioController;

    void Awake()
    {
        AudioController = GetComponent<AudioController>();
    }

    void Update()
    {
        MuteButtonText.text = AudioController.IsMuted ? "Unmute audio" : "Mute audio";
    }

    public void OnClickAudio() 
    {
        AudioController.ToggleMute();
    }

    public void OnClickRestart() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}