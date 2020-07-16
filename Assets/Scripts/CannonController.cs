using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject Shot;
    public float Damage = 10f;
    public void Fire(Vector2 direction, Vector2 baseVelocity)
    {
        ShotController shot = Instantiate(Shot, transform.position, transform.rotation, transform.parent).GetComponent<ShotController>();
        shot.Fire(direction, baseVelocity, Damage);
    }
}