using UnityEngine;
using DG.Tweening;

public class SpaceStationController : MonoBehaviour
{
    private const float MaxAnimationDuration = 6.1f;

    public bool ShouldAnimate = true;

    private VisibleObject VisibleObject;
    private float AnimationDuration = MaxAnimationDuration;

    void Awake()
    {
        VisibleObject = GetComponent<VisibleObject>();
    }

    void Update()
    {
        if(ShouldAnimate && VisibleObject.IsVisible)
        {
            AnimationDuration -= Time.deltaTime;
            if(AnimationDuration <= 0)
            {
                Animate();
                AnimationDuration = MaxAnimationDuration;
            }
        }
    }

    void Animate()
    {
        transform
				.DOMoveY(transform.position.y + 0.75f, 3f)
				.SetEase(Ease.OutQuad)
				.OnComplete(() =>
				{	
					transform
						.DOMoveY(transform.position.y - 0.75f, 3f)
						.SetEase(Ease.OutQuad);
				});
    }
}