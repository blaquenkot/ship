using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpecialGaugeController : MonoBehaviour
{
    public Sprite Image0;
    public Sprite Image1;
    public Sprite Image2;
    public Sprite Image3;
    public Sprite Image4;
    public Sprite Image5;
    public Sprite Image6;
    public Sprite Image7;
    public Sprite Image8;
    public Sprite Image9;
    public Sprite Image10;
    public Sprite ImageOff;
    public Sprite ImageOn;

    public Image GaugeImage;
    public Text ButtonText;

    private Sprite CurrentImage1;
    private Sprite CurrentImage2;
    private float FlickTimer = 0.5f;
    private float Value = 0f;
    private Vector3 BaseScale = Vector3.one;

    void Start()
    {
        BaseScale = transform.localScale;
    }

    void Update()
    { 
        FlickTimer -= Time.deltaTime;
        if (FlickTimer <= 0f)
        {
            if (GaugeImage.sprite == CurrentImage1)
            {
                GaugeImage.sprite = CurrentImage2;
                FlickTimer = 0.7f;
            }
            else
            {
                GaugeImage.sprite = CurrentImage1;
                FlickTimer = 0.2f;
            }
 
            if (Value == 1f) 
            {
                ButtonText.enabled = true;

                if (transform.localScale == BaseScale) 
                {
                    transform.DOScale(BaseScale * 1.1f, 0.5f).OnComplete(() => {
                        transform.DOScale(BaseScale, 0.25f);
                    });
                }
            } 
            else 
            {
                ButtonText.enabled = false;
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
        Value = value;

        if(value == 0f)
        {
            UpdateCurrentImages(Image0, Image0);
        }
        else if(value <= 0.1f)
        {
            UpdateCurrentImages(Image1, Image1);
        }
        else if(value <= 0.2f)
        {
            UpdateCurrentImages(Image2, Image2);
        }
        else if (value <= 0.3f)
        {
            UpdateCurrentImages(Image3, Image3);
        }
        else if (value <= 0.4f)
        {
            UpdateCurrentImages(Image4, Image4);
        }
        else if (value <= 0.5f)
        {
            UpdateCurrentImages(Image5, Image5);
        }
        else if (value <= 0.6f)
        {
            UpdateCurrentImages(Image6, Image6);
        }
        else if (value <= 0.7f)
        {
            UpdateCurrentImages(Image7, Image7);
        }
        else if (value < 0.8f)
        {
            UpdateCurrentImages(Image8, Image8);
        }
        else if (value <= 0.9f)
        {
            UpdateCurrentImages(Image9, Image9);
        }
        else if (value < 1f)
        {
            UpdateCurrentImages(Image10, Image10);
        }
        else 
        {
            UpdateCurrentImages(ImageOff, ImageOn);
        }
    }
}
