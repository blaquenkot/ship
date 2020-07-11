using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour, IGauge
{
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}
