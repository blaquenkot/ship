using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour, IGauge
{
    private float minAngle = 10;
    private float maxAngle = 170;
    public Sprite Image0;
    public Sprite Image1;
    public Sprite Image2;
    public Sprite Image3;
    public Sprite Image4;
    public Sprite Image5;


    public GameObject GaugeLight;
    private UnityEngine.UI.Image GaugeLightImage;
    public void Start()
    {
        GaugeLightImage = GaugeLight.GetComponent<UnityEngine.UI.Image>();
    }


    public void SetValue(float value)
    {
        if(value< 0.20f)
        {
            GaugeLightImage.sprite = Image0;
        }
       else if (value < 0.35f)
        {
            GaugeLightImage.sprite = Image1;
        }
        else if (value < 0.50f)
        {
            GaugeLightImage.sprite = Image2;
        }
        else if (value < 0.65f)
        {
            GaugeLightImage.sprite = Image3;
        }
        else if (value < 0.80f)
        {
            GaugeLightImage.sprite = Image4;
        }
        else 
        {
            GaugeLightImage.sprite = Image5;
        }
    }
        }
