using UnityEngine;

public enum PowerUpType { Acceleration, Torque, Shoot, Shield }

public class PowerUpController : MonoBehaviour
{
    public PowerUpType Type = PowerUpType.Acceleration;
    public float Amount = 0.25f;

    private AudioClip Sound;
    private Transform Border;

    void Awake()
    {
        Sound = Resources.Load<AudioClip>("powerup");
        Border = transform.GetChild(0);
    }

    void FixedUpdate()
    {
        Border.rotation = Quaternion.AngleAxis(Border.eulerAngles.z + 1f, Vector3.forward);
    }
    
    public void Consume()
    {
        AudioSource.PlayClipAtPoint(Sound, transform.position);
        Destroy(gameObject);
    }
}