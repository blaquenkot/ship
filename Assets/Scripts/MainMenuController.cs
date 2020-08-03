using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Text MuteButtonText;

    private AudioController AudioController;

    void Awake()
    {
        AudioController = GetComponent<AudioController>();
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
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClickSound() 
    {
        AudioController.ToggleMute();
    }
}

