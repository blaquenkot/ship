using UnityEngine;

public enum PowerUpType { Acceleration, Torque, Shoot, Shield }

public class PowerUpController : MonoBehaviour
{
    public PowerUpType Type = PowerUpType.Acceleration;
    public float Amount = 0.25f;

    public void Consume()
    {
        Destroy(gameObject);
    }
}