using UnityEngine;

public class GaugeController : MonoBehaviour
{
    public GameObject aguja;
    private const float turningRate = 5f;    
    private const float minAngle = 15;
    private const float maxAngle = 165;    
    private float Value = 0f;

    void Update()
    {
        var angle = 90 - Mathf.LerpAngle(minAngle, maxAngle, Value + Random.Range(-0.02f, 0.02f)); 
        aguja.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetValue(float value)
    {
        Value = value;   
    }
}
