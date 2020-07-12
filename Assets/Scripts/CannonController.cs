using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject Shot;
    public float Damage = 10f;
    public void Fire(Vector2 direction)
    {
        ShotController shot = Instantiate(Shot, transform.position, transform.rotation, transform.parent).GetComponent<ShotController>();
        shot.Fire(direction, Damage);
    }
}