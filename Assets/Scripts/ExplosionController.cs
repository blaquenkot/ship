using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private float Timer = 0.5f;

    void FixedUpdate() 
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}