using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
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

    void UpdateCurrentImages(Sprite image1, Sprite image2)
    {
        CurrentImage1 = image1;
        CurrentImage2 = image2;
    }

    public void SetValue(float value)
    {
        if(value< 0.20f)
        {
            UpdateCurrentImages(ImageOff, ImageOn);
        }
        else if (value < 0.35f)
        {
            UpdateCurrentImages(Image1, Image1);
        }
        else if (value < 0.50f)
        {
            UpdateCurrentImages(Image2, Image2);
        }
        else if (value < 0.65f)
        {
            UpdateCurrentImages(Image3, Image3);
        }
        else if (value < 0.80f)
        {
            UpdateCurrentImages(Image4, Image4);
        }
        else 
        {
            UpdateCurrentImages(Image5, Image5);
        }

        FlickTimer = 0f;
    }
}
