using UnityEngine;
using UnityEngine.UI;

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
}