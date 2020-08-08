using UnityEngine;
using DG.Tweening;
public class BurstController : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color burstColor = spriteRenderer.color;
        burstColor.a = 0.25f;
        DOTween.Sequence()
            .Join(transform.DOScale(Vector3.one*1.25f, 0.3f))
            .Join(spriteRenderer.DOColor(burstColor, 0.3f))
            .OnComplete(()=> {
                Destroy(gameObject);
            });
    }
}