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
        IKilleable killeable = collision.collider.gameObject.GetComponent<IKilleable>();
        if(killeable != null) 
        {
            killeable.Kill();
            if(WorldController) 
            {
                WorldController.AddPoints(1);

                if(killeable.IsEnemy()) 
                {
                    WorldController.EnemyKilled();
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