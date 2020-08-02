using UnityEngine;
using DG.Tweening;

public enum PowerUpType { Acceleration, Torque, Shoot, Shield }

public class PowerUpController : MonoBehaviour
{
    public Transform Border;
    public PowerUpType Type = PowerUpType.Acceleration;
    public float Amount = 0.25f;

    void Awake()
    {
        Border = transform.GetChild(0);
    }

    void Start()
    {
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.DOColor(color, 0.75f);
        }
    }

    void FixedUpdate()
    {
        Border.rotation = Quaternion.AngleAxis(Border.eulerAngles.z + 1f, Vector3.forward);
    }
}