using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public event Action onClickHandler = null;
    public event Action onPressedHandler = null; // IDragHandler 사용해도 되지 않나?
    public event Action onPointerDownHandler = null;
    public event Action onPointerUpHandler = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDownHandler?.Invoke();

        if (onPressedHandler != null && pressedRoutine == null)
        {
            pressedRoutine = StartCoroutine(PressedRoutine());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onPressedHandler != null && pressedRoutine != null)
        {
            StopCoroutine(pressedRoutine);
            pressedRoutine = null;
        }

        onPointerUpHandler?.Invoke();
    }

    private Coroutine pressedRoutine;
    private IEnumerator PressedRoutine()
    {
        while (true)
        {
            onPressedHandler?.Invoke();
            yield return null;
        }
    }
}
