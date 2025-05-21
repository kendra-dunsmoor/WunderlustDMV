using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class OnHoverUnderlineText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private string text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        textObject.text = "<u>" + text + "</u>";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textObject.text = text;
    }
}
