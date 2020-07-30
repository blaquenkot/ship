using UnityEngine;
using DG.Tweening;

public class ExplosionController : MonoBehaviour
{
    void Start() 
    {        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = 0.5f;

        DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.5f, 0.75f))
                .Join(spriteRenderer.DOColor(color, 0.75f))
                .Play()
                .OnComplete(() => {
                    Destroy(gameObject);
                });
    }
}