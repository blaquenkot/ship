using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    private ShipController Player;

    private float MaxRotation = 100;

    void Start() 
    {
        Player = UnityEngine.Object.FindObjectOfType<ShipController>();
    }

    void FixedUpdate()
    {
        RotateTowardsPlayer();
    }

    public void TakeDamage(float damageTaken)
    {
        Destroy(gameObject);
    }

    private void RotateTowardsPlayer()
    {
        var direction = transform.rotation * Vector2.right;
        var diffVector = Player.transform.position - transform.position;
        var angleDiff = Vector2.SignedAngle(direction, diffVector);
        var clampedDiff = Mathf.Clamp(
            angleDiff,
            -MaxRotation * Time.deltaTime,
            MaxRotation * Time.deltaTime
        );

        transform.rotation = Quaternion.AngleAxis(
            transform.eulerAngles.z + clampedDiff,
            Vector3.forward
        );
    }
}
