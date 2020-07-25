using UnityEngine;

public class OrbController : MonoBehaviour 
{
    public void Consume()
    {
        //AudioSource.PlayClipAtPoint(Sound, transform.position);
        Destroy(gameObject);
    }
}