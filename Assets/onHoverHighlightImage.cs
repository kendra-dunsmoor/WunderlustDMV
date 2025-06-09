using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class onHoverHighlightImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private Image image;
    private Color originalColor;
    public float lightnessFactor = 0.3f; // Adjust this to control the amount of lightening

    void Start()
    {
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = originalColor + new Color(lightnessFactor, lightnessFactor, lightnessFactor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
    }
}
