using UnityEngine;
using UnityEngine.UI;

public class NewHighscoreController : MonoBehaviour
{
    public InputField NameInput;
    public NetworkingController NetworkingController;

    private int Points = 0;

    public void Show(int points)
    {
        Points = points;
        gameObject.SetActive(true);
    }

    public void OnClickSend()
    {
        NetworkingController.SaveScore(NameInput.text, Points, (_) => {
            Debug.Log("saved");
            gameObject.SetActive(false);
        });
    }

    public void OnClickCancel()
    {
        gameObject.SetActive(false);
    }
}