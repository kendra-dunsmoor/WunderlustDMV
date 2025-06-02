using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnHoverChangeImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite hoverImage;
    [SerializeField] private Sprite baseImage;

    public void UpdateImages(Sprite baseImage, Sprite hoverImage) {
        this.baseImage = baseImage;
        this.hoverImage = hoverImage;
        image.sprite = baseImage;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = hoverImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = baseImage;
    }
}
