using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FurniturePageController : MonoBehaviour
{
    [SerializeField] private int FURNITURE_WILL_MODIFIER = 5;
    [SerializeField] private GameObject furniturePrefab;
    [SerializeField] private Transform furnitureParent;
    [SerializeField] private GameObject upgradesScreen;
    [SerializeField] private Furniture[] FurnitureAvailable;
    private AudioManager audioManager;
    private GameManager gameManager;

    // Purchase screen:
    [SerializeField] private GameObject opaqueScreen;
    [SerializeField] private GameObject purchasePopUp;
    [SerializeField] private TextMeshProUGUI purchaseDescription;

    void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
        // These should probably get moved to prefab script but leaving all pop up logic in here for now:
        opaqueScreen.SetActive(false);
        purchasePopUp.SetActive(false);

        // Get Game Manager to check and add furniture
        gameManager = FindFirstObjectByType<GameManager>();
        // check if player has furniture and mark as purchased
        List<Furniture> playerfurniture = gameManager.FetchFurniture();
        // Instantiate all available furniture for sale. For now just going to serialize a field for this
        foreach (Furniture furniture in FurnitureAvailable) {
            GameObject furnitureUI = Instantiate(furniturePrefab, furnitureParent);
            furnitureUI.GetComponent<FurnitureUIController>().AddFurniture(furniture);
            if (playerfurniture != null && playerfurniture.Contains(furniture)) furnitureUI.GetComponent<FurnitureUIController>().MarkAsPurchased();
        }
    }

    public void Buyfurniture(FurnitureUIController furnitureUI)
    {
        Furniture furniture = furnitureUI.furniture;
        if (gameManager.FetchSoulCredits() >= furniture.price)
        {
            if (audioManager != null) audioManager.PlaySFX(audioManager.buyUpgrade);
            gameManager.AddFurniture(furniture);
            gameManager.UpdateSoulCredits(-furniture.price);
            GameObject.FindGameObjectWithTag("Counter_SoulCredits")
                .GetComponent<CurrencyCounter>()
                .RefreshCounter();
            furnitureUI.MarkAsPurchased();
        }
        else
        {
            // Not enough money
            if (audioManager != null) audioManager.PlaySFX(audioManager.noEnergy);
        }
        gameManager.UpdateMaxWill(gameManager.FetchWill() + FURNITURE_WILL_MODIFIER);
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

    public void PurchasePopUp(Furniture furniture)
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        // Add opaque background
        opaqueScreen.SetActive(true);
        // Add purchase popup
        purchasePopUp.SetActive(true);
        purchasePopUp.GetComponentInChildren<FurnitureUIController>().AddFurniture(furniture);
        purchaseDescription.text = "Buy " + furniture.title + " for " + furniture.price + " Chthonic Credits?";
    }

    public void Cancel() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        purchasePopUp.SetActive(false);
        opaqueScreen.SetActive(false);
    }
}
