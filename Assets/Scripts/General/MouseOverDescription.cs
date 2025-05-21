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
        //Fetch the text component from the GameObject
        spawnedPopUp = Instantiate(popUpBox, gameObject.transform);
        spawnedPopUp.GetComponentInChildren<TextMeshProUGUI>().text = description;
        spawnedPopUp.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        spawnedPopUp.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        spawnedPopUp.SetActive(false);
    }

    public void UpdateDescription(string description)
    {
        this.description = description;

        // TODO: weird runtime bug where this is running before start and pop up doesn't exist
        if (spawnedPopUp == null) spawnedPopUp = Instantiate(popUpBox, gameObject.transform);
        spawnedPopUp.GetComponentInChildren<TextMeshProUGUI>().text = description;
        spawnedPopUp.SetActive(false);
    }
}
