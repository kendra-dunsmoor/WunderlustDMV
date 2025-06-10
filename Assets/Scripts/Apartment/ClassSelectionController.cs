using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassSelectionController : MonoBehaviour
{
    [SerializeField] private List<Class> availableClasses;
    [SerializeField] private GameObject classOptionPrefab;
    [SerializeField] private Transform classOptionGrid;
    [SerializeField] private Sprite selectedButton;
    [SerializeField] private Sprite normalButton;

    [SerializeField] private GameObject upgradesScreen;
    private AudioManager audioManager;
    private GameManager gameManager;

    // Selection screen:
    [SerializeField] private GameObject opaqueScreen;
    [SerializeField] private GameObject purchasePopUp;
    [SerializeField] private TextMeshProUGUI purchaseDescription;

    private int selectedClass;
    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        opaqueScreen.SetActive(false);
        purchasePopUp.SetActive(false);
        AddClassOptions();
    }

    private void AddClassOptions()
    {
        // Check active class and mark as active
        Class activeClass = gameManager.FetchPlayerClass();
        if (activeClass == null) activeClass = availableClasses[0];
        int index = 0;
        foreach (Class playerClass in availableClasses)
        {
            int tempInt = index;// Weird bug where AddListener takes reference instead of value
            GameObject classUI = Instantiate(classOptionPrefab, classOptionGrid);
            classUI.GetComponentInChildren<Button>().onClick.AddListener(() => PurchasePopUp(playerClass, tempInt));
            classUI.GetComponent<ClassUIController>().AddClass(playerClass);
            if (activeClass != null && activeClass == playerClass)
            {
                classUI.GetComponentInChildren<Image>().sprite = selectedButton;
            }
            index++;
        }

    }

    public void SelectClass()
    {
        gameManager.UpdatePlayerClass(availableClasses[selectedClass]);
        // TODO: Mark as active class
        int index = 0;
        foreach (Button button in classOptionGrid.GetComponentsInChildren<Button>())
        {
            if (index == selectedClass) button.gameObject.GetComponent<Image>().sprite = selectedButton;
            else button.gameObject.GetComponent<Image>().sprite = normalButton;
            index++;
        }
        Cancel();
    }

    public void PurchasePopUp(Class playerClass, int index)
    {
        selectedClass = index;
        Debug.Log("Selected class = " + index);
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        // Add opaque background
        opaqueScreen.SetActive(true);
        // Add purchase popup
        purchasePopUp.SetActive(true);
        purchaseDescription.text = "Confirm class selection as " + playerClass.className + "?";
    }

    public void ReturnToUpgrades()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Instantiate(upgradesScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    public void CloseComputer()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Destroy(gameObject);
    }
    
    public void Cancel() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        purchasePopUp.SetActive(false);
        opaqueScreen.SetActive(false);
    }
}
