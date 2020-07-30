using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
 
public class FlashingLight : MonoBehaviour
{
    private Light2D Light;
    private float DefaultIntensity;
    private bool ShouldFlash = false;
    private bool ShouldReturnToDefaultIntensity = false;
    private float FlashDuration = 0.25f;
    private float Force = 50f;
    private float FlashTimer = 0f;

    void Start()
    {
        Light = GetComponent<Light2D>();
        DefaultIntensity = Light.intensity;
    }

    void Update()
    {
        if(ShouldFlash)
        {
            FlashTimer -= Time.deltaTime;
            if(FlashTimer > 0f)
            {
                if(!ShouldReturnToDefaultIntensity) 
                {
                    Light.intensity = Mathf.Lerp(Light.intensity, 10f, Time.deltaTime * Force);
                } 
                else
                {
                    Light.intensity = Mathf.Lerp(Light.intensity, DefaultIntensity, Time.deltaTime * Force);
                }
            } 
            else 
            {
                if(!ShouldReturnToDefaultIntensity)
                {
                    ShouldReturnToDefaultIntensity = true;
                    FlashTimer = FlashDuration;
                }
                else
                {
                    Light.intensity = DefaultIntensity;
                    ShouldFlash = false;
                }
            }
        }
    }

    public void MakeFlash(float duration = 0.05f, float force = 50f)
    {
        FlashDuration = duration;
        FlashTimer = FlashDuration;
        Force = force;
        ShouldReturnToDefaultIntensity = false;
        ShouldFlash = true;
    }
}