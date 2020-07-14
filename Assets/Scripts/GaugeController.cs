using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour, IGauge
{
    private float minAngle = 15;
    private float maxAngle = 165;

    public GameObject aguja;

    public void SetValue(float value)
    {
        var angle = Mathf.Lerp(minAngle, maxAngle, value);
        aguja.transform.eulerAngles = new Vector3(0, 0, 90 - angle);
    }
}
