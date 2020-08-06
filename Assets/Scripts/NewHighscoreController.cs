using UnityEngine;
using UnityEngine.UI;

public class NewHighscoreController : MonoBehaviour
{
    public InputField NameInput;
    public Button SendButton;
    public Text ResultText;

    public NetworkingController NetworkingController;
    public PersistentDataController PersistentDataController;

    private int Time = 0;
    private int Points = 0;

    void Awake()
    {
        if(PersistentDataController.UserName != null) 
        {
            NameInput.text = PersistentDataController.UserName;
        }
    }

    public void UpdateInfo(int points, float time)
    {
        Points = points;
        Time = (int)time;
    }

    public void OnClickSend()
    {
        string name = NameInput.text;
        if(name != null && name != "")
        {
            NetworkingController.SaveScore(name, Points, Time, (result) => {
                Debug.Log("saved");
                PersistentDataController.UserName = name;
                SendButton.gameObject.SetActive(false);
                NameInput.gameObject.SetActive(false);
                ResultText.text = "Your position: " + result["position"];
                ResultText.gameObject.SetActive(true);
            });
        }
    }
}