using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum MissionTargetState { NotVisible, Ready, Completed, Lost }
public class MissionTargetController : MonoBehaviour
{
    public MissionTargetState State { private set; get; }
    public Sprite ReadySprite;
    public Sprite CompletedSprite;
    public Sprite LostSprite;

    private Image Image;
    private bool ShouldBlink = false;
    private float BlinkCooldown = 0.2f;

    void Awake()
    {
        Image = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        if(ShouldBlink)
        {
            BlinkCooldown -= Time.deltaTime;
            if(BlinkCooldown <= 0f)
            {
                Color color = Image.color;
                color.a = color.a == 0f ? 1f : 0f;
                Image.DOColor(color, 0.15f);
                
                BlinkCooldown = 0.2f;
            }
        }
    }

    void Pulse(int times)
    {
        Vector3 originalScale = transform.localScale;
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < times; i++)
        {
            sequence.Append(transform.DOScale(originalScale * 1.25f, 0.1f));
            sequence.Append(transform.DOScale(originalScale, 0.1f));
        }
        sequence.Play();
    }

    void Appear()
    {
        Color color = Image.color;
        color.a = 0.0f;
        Image.color = color;
        Image.enabled = true;
        color.a = 1f;
        Image.DOColor(color, 0.25f);
    }

    public void UpdateState(MissionTargetState state)
    {
        State = state;

        Sprite newSprite = null;

        switch(State)
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
            if(!Image.enabled) {
                Appear();
            }
            Pulse(4);
        }
        else
        {
            Image.enabled = false;
        }
    }

    public void ToggleBlink()
    {
        ShouldBlink = !ShouldBlink;
        if(!ShouldBlink)
        {
            Color color = Image.color;
            color.a = 1f;
            Image.DOColor(color, 0.15f);
        }
    }
}