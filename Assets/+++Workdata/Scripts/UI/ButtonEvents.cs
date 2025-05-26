using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerEnterHandler, IPointerClickHandler
{
    public event Action onSelect;
    public event Action onSubmit;
    public event Action onMouseEnter;
    public event Action onMouseClick;
    
    public void OnSelect(BaseEventData eventData)
    {
        onSelect?.Invoke();
    }

    public void OnSubmit(BaseEventData eventData)
    {
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
