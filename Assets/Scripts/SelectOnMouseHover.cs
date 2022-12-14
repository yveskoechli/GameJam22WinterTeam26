using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class SelectOnMouseHover : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{

    private Selectable selectable;


    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        if (selectable == null)
        {
            Debug.LogWarning("No selectable found on Game Object", this);
        }
    }

    public void OnPointerEnter(PointerEventData eventdata)
    {
        if (!selectable.interactable)
        {
            return;
        }

        if (EventSystem.current.alreadySelecting)
        {
            return;
        }
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!selectable.interactable)
        {
            return;
        }
        selectable.OnPointerExit(null);
    }
}
