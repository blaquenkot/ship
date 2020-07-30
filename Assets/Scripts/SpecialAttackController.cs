using UnityEngine;
using DG.Tweening;

public class SpecialAttackController : MonoBehaviour
{
    public static float Duration = 0.5f;
    private WorldController WorldController;
    private GameObject Ship;

    private Vector3 MinScale = Vector3.zero;
    private Vector3 MaxScale = Vector3.one * 30f;

    public void Fire(GameObject ship, WorldController worldController)
    {
        Ship = ship;
        WorldController = worldController;

        float internalDuration = SpecialAttackController.Duration * 0.5f;

        transform.DOScale(MaxScale, internalDuration).OnComplete(() => {
            transform.DOScale(MinScale, internalDuration).OnComplete(() => {
                Destroy(gameObject);
            });
        });
    }
    
    void OnCollisionEnter2D(Collision2D collision) 
    {
        IDamageable damageable = collision.collider.gameObject.GetComponent<IDamageable>();
        if(damageable != null) 
        {
            damageable.TakeDamage(9999999);
            if(WorldController) 
            {
                WorldController.AddPoints(1);

                if(damageable.IsEnemy()) 
                {
                    WorldController.EnemyKilled(true);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        PowerUpController powerUp = collider.GetComponent<PowerUpController>();
        if(powerUp)
        {
            powerUp.Target = Ship;
        }
    }
}