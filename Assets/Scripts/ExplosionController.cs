using UnityEngine;
using DG.Tweening;

public class ExplosionController : MonoBehaviour
{
    private float Timer = 1f;

    void Start() 
    {        
        transform.DOScale(Vector3.one, 0.3f);
    }

    void FixedUpdate() 
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}