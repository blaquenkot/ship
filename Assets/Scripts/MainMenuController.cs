using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Text MuteButtonText;
    public GameObject Highscores;

    private SceneManagerController SceneManagerController;
    private AudioController AudioController;

    void Awake()
    {
        AudioController = GetComponent<AudioController>();
        SceneManagerController = Object.FindObjectOfType<SceneManagerController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) 
        {
            OnClickStart();
        }

        MuteButtonText.text = AudioController.IsMuted ? "Unmute audio" : "Mute audio";
    }

    public void OnClickStart() 
    {
        SceneManagerController.GoToNexScene();
    }

    public void OnClickSound() 
    {
        AudioController.ToggleMute();
    }

    public void OnClickHighscores() 
    {
        Highscores.SetActive(true);
    }
}

