using Cinemachine;
using UnityEngine;

public class ShakeCameraController : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin Noise;

    private bool IsShaking = false;
    private float ShakeTime = 0f;

    void Start() 
    {
        Noise = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void FixedUpdate() {
        ShakeTime -= Time.deltaTime;

        if(ShakeTime < 0f && IsShaking) {
            UpdateNoise(0f, 0f);
            IsShaking = false;
        }
    }

    public void Shake() {
        UpdateNoise(2f, 2f);
        IsShaking = true;
        ShakeTime = 0.25f;
    }

    private void UpdateNoise(float amplitudeGain, float frequencyGain) 
    {
        Noise.m_AmplitudeGain = amplitudeGain;
        Noise.m_FrequencyGain = frequencyGain;
    }
}