using UnityEngine;
using DG.Tweening;

public class SpecialAttackController : MonoBehaviour
{
    private WorldController WorldController;
    private GameObject Ship;

    public void Fire(GameObject ship, WorldController worldController)
    {
        Ship = ship;
        WorldController = worldController;

        transform.DOScale(Vector3.one * 50f, 0.5f).OnComplete(() => {
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => {
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