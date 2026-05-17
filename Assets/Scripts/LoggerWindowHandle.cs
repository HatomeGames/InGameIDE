using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoggerWindowHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] RectTransform loggerWindow;
    [SerializeField] LayoutElement loggerLayoutElement;
    [SerializeField] float minHeight = 25;

    bool isDragging;
    float bias = 0;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isDragging = true;
        bias = loggerLayoutElement.preferredHeight - GetLocalHeight(eventData);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isDragging = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        float height = GetLocalHeight(eventData);
        loggerLayoutElement.preferredHeight = Mathf.Max(height + bias, minHeight);
    }

    float GetLocalHeight(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(loggerWindow, eventData.position, null, out var local);
        return local.y;
    }
}
