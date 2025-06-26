using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip buttonSubmitSound;
    
    public event Action onSelect;
    public event Action onSubmit;
    public event Action onMouseEnter;
    public event Action onMouseClick;

    public void OnSelect(BaseEventData eventData)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayUISFX(MusicManager.Instance.buttonHover);
        }
        onSelect?.Invoke();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (buttonSubmitSound != null)
        {
            MusicManager.Instance.PlayUISFX(buttonSubmitSound);
        }
        else
        {
            MusicManager.Instance.PlayUISFX(MusicManager.Instance.buttonPress);
        }
        onSubmit?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onMouseClick?.Invoke();
    }
}
