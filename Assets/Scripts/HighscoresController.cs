using UnityEngine;
using UnityEngine.UI;

public class HighscoresController : MonoBehaviour
{
    public Text HighscoresText;
    private NetworkingController NetworkingController;

    void Awake()
    {
        NetworkingController = GetComponent<NetworkingController>();

        NetworkingController.GetHighscores((list) => {
            HighscoresText.text = "";
            foreach (var item in list)
            {
                HighscoresText.text += item.Value["name"] + ": " + item.Value["score"] + "\n";
            }
        });
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}