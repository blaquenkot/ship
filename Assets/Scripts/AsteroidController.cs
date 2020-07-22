using UnityEngine;
using DG.Tweening;

public class AsteroidController : MonoBehaviour, IDamageable
{
    public float Health = 5f;
    public float Velocity = 2.5f;

    void Start() 
    {
        transform.DOScale(Vector3.one, 0.75f);

        ShipController Player = UnityEngine.Object.FindObjectOfType<ShipController>();
        GetComponent<Rigidbody2D>().velocity = (Player.transform.position - transform.position).normalized * Velocity;
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

    private void Destroyed()
    {
        Destroy(gameObject);
    }
}