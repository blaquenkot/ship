using UnityEngine;
using DG.Tweening;
public class CannonController : MonoBehaviour
{
    public GameObject Shot;
    public GameObject Burst;

    public WorldController WorldController;
    public float Damage = 10f;
    
    public void Fire(Vector2 direction, Vector2 baseVelocity)
    {
        Instantiate(Burst, transform.position, transform.rotation, transform.parent);
        ShotController shot = Instantiate(Shot, transform.position, transform.rotation, transform.parent).GetComponent<ShotController>();
        shot.WorldController = WorldController;
        shot.Fire(direction, baseVelocity, Damage);
    }
}