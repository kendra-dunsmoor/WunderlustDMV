using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VendingMachineController : MonoBehaviour
{
    [SerializeField] private Transform itemsGridParent;
    [SerializeField] private GameObject itemPrefab;

    private AudioManager audioManager;
    private GameManager gameManager;

    // Purchase screen:
    [SerializeField] private GameObject opaqueScreen;
    [SerializeField] private GameObject purchasePopUp;
    [SerializeField] private TextMeshProUGUI purchaseDescription;
    [SerializeField] private TextMeshProUGUI purchaseFlavorText;

    void Start()
    {
        // These should probably get moved to prefab script but leaving all pop up logic in here for now:
        opaqueScreen.SetActive(false);
        purchasePopUp.SetActive(false);

        audioManager = FindFirstObjectByType<AudioManager>();

        // Get Game Manager for updating inventory
        gameManager = FindFirstObjectByType<GameManager>();

        // Fill machine with items
        List<Item> itemsForSale = gameManager.FetchRandomItems(4);
        foreach (Item item in itemsForSale) {
            GameObject itemUI = Instantiate(itemPrefab, itemsGridParent);
            itemUI.GetComponent<ItemUIController>().AddItem(item);
        }
    }

    public void BuyItem(ItemUIController itemUI)
    {
        Item item = itemUI.item;
        if (gameManager.FetchOfficeBucks() >= item.price) {
            if (audioManager != null) audioManager.PlaySFX(audioManager.buyUpgrade);
            if (item is UsableItem)
                gameManager.AddToInventory(item.ID);
            else
                gameManager.AddArtifact(item.ID);
            gameManager.UpdateOfficeBucks(-item.price);
            // itemUI.MarkAsPurchased();
            // TODO: need to get reference to the original UI, not the popup image
            CancelPurchase();
        }
    }

    public void NextShift()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void PurchasePopUp(Item item)
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        // Add opaque background
        opaqueScreen.SetActive(true);
        // Add purchase popup
        purchasePopUp.SetActive(true);
        purchasePopUp.GetComponentInChildren<ItemUIController>().AddItemUIForPurchase(item);
        purchaseDescription.text = "Buy " + item.itemName + " for " + item.price + " office bucks?";
        purchaseFlavorText.text = item.flavorText;
    }

    public void CancelPurchase() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        purchasePopUp.SetActive(false);
        opaqueScreen.SetActive(false);
    }
}
