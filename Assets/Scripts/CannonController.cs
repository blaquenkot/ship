using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject Shot;
    public WorldController WorldController;
    public float Damage = 10f;
    public void Fire(Vector2 direction, Vector2 baseVelocity)
    {
        ShotController shot = Instantiate(Shot, transform.position, transform.rotation, transform.parent).GetComponent<ShotController>();
        shot.WorldController = WorldController;
        shot.Fire(direction, baseVelocity, Damage);
    }
}