using UnityEngine;
using DG.Tweening;

public class AsteroidController : MonoBehaviour, IDamageable
{
    public float Health = 5f;
    public float Velocity = 2.5f;

    void Start() 
    {
        float size = Random.Range(0.75f, 1.5f);
        transform.DOScale(Vector3.one * size, 0.75f);

        ShipController Player = UnityEngine.Object.FindObjectOfType<ShipController>();
        Vector2 direction = (Player.transform.position - transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0)).normalized;
        GetComponent<Rigidbody2D>().velocity = direction * Velocity;
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.AngleAxis(transform.eulerAngles.z + 0.5f, Vector3.forward);
    }

    public bool TakeDamage(float damageTaken)
    {
        Health -= damageTaken;

        if(Health <= 0)
        {
            Destroyed();
            return true;
        } 
        else 
        {
            return false;
        }
    }

    public bool IsEnemy()
    {
        return false;
    }
    
    private void Destroyed()
    {
        Destroy(gameObject);
    }
}