using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum MissionTargetState { NotVisible, Ready, Completed, Lost }
public class MissionTargetController : MonoBehaviour
{
    public Sprite ReadySprite;
    public Sprite CompletedSprite;
    public Sprite LostSprite;

    private Image Image;

    void Awake()
    {
        Image = GetComponent<Image>();
    }

    void Pulse()
    {
        Vector3 originalScale = transform.localScale;
        transform.DOScale(originalScale * 1.25f, 0.1f).OnComplete(() => {
            transform.DOScale(originalScale, 0.1f);
        }); 
    }

    public void UpdateState(MissionTargetState state)
    {
        Sprite newSprite = null;

        switch(state)
        {
            case MissionTargetState.NotVisible:
            {
                break;
            }
            case MissionTargetState.Ready:
            {
                newSprite = ReadySprite;
                break;
            }
            case MissionTargetState.Completed:
            {
                newSprite = CompletedSprite;
                break;
            }
            case MissionTargetState.Lost:
            {
                newSprite = LostSprite;
                break;
            }
        }

        if(newSprite)
        {
            Image.sprite = newSprite;
            Image.enabled = true;
            Pulse();
        }
        else
        {
            Image.enabled = false;
        }
    }
}