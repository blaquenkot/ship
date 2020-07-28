using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour, IGauge
{
    public Sprite ImageOn;
    public Sprite ImageOff;
    public Sprite Image1;
    public Sprite Image2;
    public Sprite Image3;
    public Sprite Image4;
    public Sprite Image5;
    public Image GaugeLightImage;
    private Sprite CurrentImage1;
    private Sprite CurrentImage2;
    private float FlickTimer = 0.5f;

    void Update()
    { 
        FlickTimer -= Time.deltaTime;
        if (FlickTimer <= 0f)
        {
            if (GaugeLightImage.sprite == CurrentImage1)
            {
                GaugeLightImage.sprite = CurrentImage2;
                FlickTimer = 0.7f;
            }
            else
            {
                GaugeLightImage.sprite = CurrentImage1;
                FlickTimer = 0.2f;
            }
 
        }
    }


    public void SetValue(float value)
    {
        if(value< 0.20f)
        {
            CurrentImage1 = ImageOff;
            CurrentImage2 = ImageOn;
        }
       else if (value < 0.35f)
        {
            CurrentImage1 = Image1;
            CurrentImage2 = Image1;
        }
        else if (value < 0.50f)
        {
            CurrentImage1 = Image2;
            CurrentImage2 = Image2;
        }
        else if (value < 0.65f)
        {
            CurrentImage1 = Image3;
            CurrentImage2 = Image3;
        }
        else if (value < 0.80f)
        {
            CurrentImage1 = Image4;
            CurrentImage2 = Image4;
        }
        else 
        {
            CurrentImage1 = Image5;
            CurrentImage2 = Image5;
        }
    }
        }
