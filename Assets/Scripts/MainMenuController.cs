using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnClickStart() {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClickSound() {
        if(AudioListener.volume == 1f) {
            AudioListener.volume = 0f;
        } else {
            AudioListener.volume = 1f;
        }
    }
}

