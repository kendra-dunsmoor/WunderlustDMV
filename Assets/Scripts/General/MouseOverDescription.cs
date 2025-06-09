using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //When the mouse hovers over the GameObject, it displays text popup
    [SerializeField] private GameObject popUpBox;
    [TextArea(3,10)]
    [SerializeField] private string description;
    private GameObject spawnedPopUp;
    private Transform originalTransform;
    private bool firstEnter;

    void Start()
    {
        this.originalTransform = this.transform;
        if (popUpBox != null)
        {
            //spawnedPopUp = Instantiate(popUpBox, transform);

            spawnedPopUp = Instantiate(popUpBox, GameObject.FindGameObjectWithTag("Canvas").transform);
            spawnedPopUp.SetActive(false);
        }
        // testing:
        firstEnter = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("In pointer enter");
        if (spawnedPopUp != null && !firstEnter)
        {
            Debug.Log("First enter");
            spawnedPopUp.transform.SetAsLastSibling();
            spawnedPopUp.transform.position = this.originalTransform.position;
            spawnedPopUp.SetActive(true);
            spawnedPopUp.GetComponentInChildren<TextMeshProUGUI>().text = description;
            firstEnter = true;
        }
        else if (spawnedPopUp != null && firstEnter)
        {
            Debug.Log("Not first enter");
            spawnedPopUp.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("In pointer exit");
        if (spawnedPopUp != null) spawnedPopUp.SetActive(false);
    }

    public void UpdateDescription(string description, string title = "")
    {
        if (title != "") this.description = "<u>" + title + "</u>\n" + description;
        else this.description = description;
    }
}
