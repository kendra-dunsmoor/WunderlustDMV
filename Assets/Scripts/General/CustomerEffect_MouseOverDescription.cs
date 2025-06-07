using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomerEffect_MouseOverDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Probably a better way but testing this for the issue with effects on customer mini canvas
    //When the mouse hovers over the GameObject, it displays text popup
    [SerializeField] private GameObject popUpBox;
    [TextArea(3,10)]
    [SerializeField] private string description;
    private GameObject spawnedPopUp;
    private Transform originalTransform;

    void Start()
    {
        this.originalTransform = this.transform;
        if (popUpBox != null) {            
            spawnedPopUp = Instantiate(popUpBox, originalTransform);
            spawnedPopUp.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (spawnedPopUp != null)
        {
            spawnedPopUp.transform.SetAsLastSibling();
            spawnedPopUp.SetActive(true);           
            spawnedPopUp.GetComponentInChildren<TextMeshProUGUI>().text = description;
          
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (spawnedPopUp != null) spawnedPopUp.SetActive(false);
    }

    public void UpdateDescription(string description, string title = "")
    {
        if (title != "") this.description = "<u>" + title + "</u>\n" + description;
        else this.description = description;
    }
}
