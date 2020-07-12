using UnityEngine;

public enum PowerUpType { Acceleration, Torque, Shoot, Shield }

public class PowerUpController : MonoBehaviour
{
    public PowerUpType Type = PowerUpType.Acceleration;
    public float Amount = 0.25f;

    private AudioClip Sound;

    void Awake()
    {
        Sound = Resources.Load<AudioClip>("powerup");
    }
    
    public void Consume()
    {
        AudioSource.PlayClipAtPoint(Sound, transform.position);
        Destroy(gameObject);
    }
}