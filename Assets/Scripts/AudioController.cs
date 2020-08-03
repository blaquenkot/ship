using UnityEngine;

public class AudioController : MonoBehaviour
{
    public bool IsMuted {
        get {
            return (AudioListener.volume == 0f);
        }
    }
    
    public void ToggleMute()
    {
        if(AudioListener.volume == 1f) {
            AudioListener.volume = 0f;
        } 
        else
        {
            AudioListener.volume = 1f;
        }
    }
}