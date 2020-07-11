using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour, IGauge
{
    private Slider slider;

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}
