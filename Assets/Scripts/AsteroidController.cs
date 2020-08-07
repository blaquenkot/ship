using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using DG.Tweening;

public class AsteroidController : MonoBehaviour, IDamageable
{
    private const float DestructionDuration = 1f;

    public float Health = 5f;
    public float Velocity = 2.5f;

    private Light2D Light;
    private Rigidbody2D Rigidbody;
    private SpriteRenderer SpriteRenderer;
    private SpriteRenderer[] DerbisSpriteRenderers;
    
    void Awake()
    {
        Light = GetComponentInChildren<Light2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        DerbisSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start() 
    {
        Vector3 size = Vector3.one * Random.Range(0.75f, 1.5f);
        transform.DOScale(size, 0.75f);
        foreach (var debris in DerbisSpriteRenderers)
        {   
            debris.transform.localScale = size;
        }

        ShipController Player = UnityEngine.Object.FindObjectOfType<ShipController>();
        Vector2 direction = (Player.transform.position - transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0)).normalized;
        Rigidbody.velocity = direction * Velocity;
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
        Rigidbody.velocity = Vector2.zero;
        SpriteRenderer.enabled = false;
        Light.enabled = false;
        Sequence sequence = DOTween.Sequence();
        foreach (var debris in DerbisSpriteRenderers)
        {   
            Transform debrisTransform = debris.transform;
            Vector3 newScale = debrisTransform.localScale * 0.25f;
            Vector3 newPosition = debrisTransform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
            Color newColor = debris.color;
            newColor.a = 0f;
            sequence.Join(debris.DOColor(newColor, DestructionDuration));
            sequence.Join(debrisTransform.DOScale(newScale, DestructionDuration));
            sequence.Join(debrisTransform.DOMove(newPosition, DestructionDuration));
        }
        sequence.Play().OnComplete(() => {
            Destroy(gameObject);
        });
    }
}