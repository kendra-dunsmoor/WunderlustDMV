using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnHoverChangeImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite hoverImage;
    [SerializeField] private Sprite baseImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = hoverImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = baseImage;
    }
}
