using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    void FixedUpdate()
    {
        transform.position += new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0) * Time.deltaTime;
    }

    public void TakeDamage(float damageTaken)
    {
        Destroy(gameObject);
    }
}
