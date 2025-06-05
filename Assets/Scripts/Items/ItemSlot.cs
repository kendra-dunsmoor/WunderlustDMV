using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public Image Image;

    public event Action<Item> OnClickEvent;
    private Item _item;
    public Item Item {
        get { return _item;}
        set {
            _item = value;
            if (_item == null) {
                Image.enabled = false;
            } else {
                Image.sprite = _item.Icon;
                Image.enabled = true;
                gameObject.GetComponent<MouseOverDescription>().UpdateDescription(_item.description, _item.itemName);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Left) {
            if (Item != null && OnClickEvent != null) {
                OnClickEvent(Item);
            }
        }
    }

    private void OnValidate()
    {
        if (Image == null) {
            Image = GetComponent<Image>();
        }
    }
}
