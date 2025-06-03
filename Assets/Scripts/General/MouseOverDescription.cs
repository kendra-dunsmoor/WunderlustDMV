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

    void Start()
    {
        if (popUpBox != null) {
            spawnedPopUp = Instantiate(popUpBox, transform);

            // TODO: how??????? to fix????? Need Canvas to be parent but moved to button's position so it doesn't slip behind other canvas elements
            // I'm going absolutely crazy trying to fix this messing w canvas though can't do this yet
            
            // spawnedPopUp = Instantiate(popUpBox, GameObject.FindGameObjectWithTag("Canvas").transform);
            // spawnedPopUp.GetComponent<RectTransform>().position = gameObject.GetComponent<RectTransform>().position;

            // spawnedPopUp.transform.position = transform.position + new Vector3(0, 0.5f, 0); // Position the popup
            spawnedPopUp.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (spawnedPopUp != null)
        {
            spawnedPopUp.SetActive(true);
            spawnedPopUp.GetComponentInChildren<TextMeshProUGUI>().text = description;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (spawnedPopUp != null) spawnedPopUp.SetActive(false);
    }

    public void UpdateDescription(string description)
    {
        this.description = description;
    }
}
