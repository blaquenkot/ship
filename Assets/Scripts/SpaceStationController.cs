using UnityEngine;
using DG.Tweening;

public class SpaceStationController : MonoBehaviour
{
    private VisibleObject VisibleObject;
    private float AnimationDuration = 1.1f;

    void Awake()
    {
        VisibleObject = GetComponent<VisibleObject>();
    }

    void Update()
    {
        if(VisibleObject.IsVisible)
        {
            AnimationDuration -= Time.deltaTime;
            if(AnimationDuration <= 0)
            {
                Animate();
                AnimationDuration = 1.1f;
            }
        }
    }

    void Animate()
    {
        transform
				.DOMoveY(transform.position.y + 0.25f, 0.5f)
				.SetEase(Ease.OutQuad)
				.OnComplete(() =>
				{	
					transform
						.DOMoveY(transform.position.y - 0.25f, 0.5f)
						.SetEase(Ease.InQuad);
				});
    }
}