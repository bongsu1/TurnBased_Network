using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public event Action onClickHandler = null;
    public event Action onPressedHandler = null;
    public event Action onPointerDownHandler = null;
    public event Action onPointerUpHandler = null;

    private bool interactable = true;
    public bool Interactable { get { return interactable; } set { interactable = value; } }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (interactable == false)
            return;

        onClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (interactable == false)
            return;

        onPointerDownHandler?.Invoke();

        if (onPressedHandler != null && pressedRoutine == null)
        {
            pressedRoutine = StartCoroutine(PressedRoutine());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (interactable == false)
            return;

        if (onPressedHandler != null && pressedRoutine != null)
        {
            StopCoroutine(pressedRoutine);
            pressedRoutine = null;
        }

        onPointerUpHandler?.Invoke();
    }

    // 1초 이상 눌리고 있어야 작동 되도록 기다림
    private WaitForSeconds waitPressed = new WaitForSeconds(1.0f);
    private Coroutine pressedRoutine;
    private IEnumerator PressedRoutine()
    {
        yield return waitPressed;
        while (true)
        {
            onPressedHandler?.Invoke();
            yield return null;
        }
    }
}
