using UnityEngine;

public class GaugeController : MonoBehaviour
{
    public GameObject aguja;
    private const float turningRate = 5f;    
    private const float minAngle = 15;
    private const float maxAngle = 165;    
    private float Value = 0f;
    private float Angle = 0f;
    private float Timer = 0f;
    private const float MaxTimer = 0.1f;

    void Update()
    {
        if (Timer >= 0f)
        {
            Timer += Time.deltaTime;
            if (Angle == 0f) 
            {
                Angle = 90 - Mathf.LerpAngle(minAngle, maxAngle, Value + Random.Range(-0.02f, 0.02f)); 
            }
            aguja.transform.rotation = Quaternion.AngleAxis(Mathf.LerpAngle(aguja.transform.eulerAngles.z, Angle, Timer), Vector3.forward);
            if(Timer >= MaxTimer) 
            {
                aguja.transform.rotation = Quaternion.AngleAxis(Angle, Vector3.forward);
                Angle = 0f;
                Timer = 0.001f;
            }
        }
    }

    public void SetValue(float value)
    {
        Value = value;   
    }
}
