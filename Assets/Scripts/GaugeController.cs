using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour, IGauge
{
    private float minAngle = 10;
    private float maxAngle = 170;

    public GameObject aguja;

    public void SetValue(float value)
    {
        var angle = Mathf.Lerp(minAngle, maxAngle, value);
        aguja.transform.eulerAngles = new Vector3(0, 0, 90 - angle);
    }
}
