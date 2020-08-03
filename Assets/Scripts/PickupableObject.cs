using UnityEngine;
using DG.Tweening;

public class PickupableObject : MonoBehaviour
{
    private const float Acceleration = 0.4f;

    public bool CanBePickedUpWithSpecialAttack = true;
    
    private GameObject Target;
    private AudioClip Sound;
    private float Velocity = 0f;
    private bool PickedUp = false;

    void Awake()
    {
        Sound = Resources.Load<AudioClip>("powerup");
    }

    void FixedUpdate()
    {
        if(Target)
        {
            Velocity += Acceleration;

            Vector3 targetDirection = Target.transform.position - transform.position;
            transform.position += targetDirection * Velocity * Time.deltaTime;
        }
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    public bool CanBePickedUp()
    {
        return !PickedUp;
    }

    public void PickUp(GameObject target)
    {
        PickedUp = true;
        Target = target;
        AudioSource.PlayClipAtPoint(Sound, transform.position);
        transform.DOScale(Vector3.one * 0.5f, 0.2f).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}