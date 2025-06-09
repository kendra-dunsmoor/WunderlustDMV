using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class OnHoverUnderlineText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI textObject;
    private string originalText;

    void Start()
    {
        originalText = textObject.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textObject.text = "<u>" + originalText + "</u>";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textObject.text = originalText;
    }
}
