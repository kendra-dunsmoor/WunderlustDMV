using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelectionController : MonoBehaviour
{
    [SerializeField] private List<Class> availableClasses;
    [SerializeField] private GameObject classOptionPrefab;
    [SerializeField] private Transform classOptionGrid;

    [SerializeField] private GameObject upgradesScreen;
    private AudioManager audioManager;
    private GameManager gameManager;


    void Awake()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();        
    }

    void Start()
    {
        AddClassOptions();
    }

    private void AddClassOptions()
    {
        foreach (Class playerClass in availableClasses) {
            GameObject classUI = Instantiate(classOptionPrefab, classOptionGrid);
            classUI.GetComponentInChildren<Button>().onClick.AddListener(() => SelectClass(playerClass));
            classUI.GetComponent<ClassUIController>().AddClass(playerClass);
            // TODO: Check active class and mark as active
        }
        // TODO: If in tutorial mark bottom two to as not unlocked somehow
    }

    public void SelectClass(Class playerClass)
    {
        gameManager.UpdatePlayerClass(playerClass);
        // TODO: Mark as active class
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
}
