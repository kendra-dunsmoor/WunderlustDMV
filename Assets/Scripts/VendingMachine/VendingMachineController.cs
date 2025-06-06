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
    private int activeIndex;

    void Start()
    {
        // These should probably get moved to prefab script but leaving all pop up logic in here for now:
        opaqueScreen.SetActive(false);
        purchasePopUp.SetActive(false);

        audioManager = FindFirstObjectByType<AudioManager>();

        // Get Game Manager for updating inventory
        gameManager = FindFirstObjectByType<GameManager>();

        // Fill machine with items
        List<Item> artifactsForSale = gameManager.FetchRandomItems(3, true);
        List<Item> itemsForSale = gameManager.FetchRandomItems(3, false);
        int i = 0;
        foreach (Item item in itemsForSale)
        {
            GameObject itemUI = Instantiate(itemPrefab, itemsGridParent);
            itemUI.GetComponent<ItemUIController>().AddItem(item, i);
            i++;
        }
        foreach (Item item in artifactsForSale)
        {
            GameObject itemUI = Instantiate(itemPrefab, itemsGridParent);
            itemUI.GetComponent<ItemUIController>().AddItem(item, i);
            i++;
        }
    }

    public void BuyItem(ItemUIController itemUI)
    {
        Item item = itemUI.item;
        if (gameManager.FetchOfficeBucks() >= item.price)
        {
            if (audioManager != null) audioManager.PlaySFX(audioManager.vendingMachineItem);
            if (item is UsableItem)
                gameManager.AddToInventory(item.ID);
            else
                gameManager.AddArtifact(item.ID);

            gameManager.UpdateOfficeBucks(-item.price);
            GameObject.FindGameObjectWithTag("Counter_OfficeBucks")
                .GetComponent<CurrencyCounter>()
                .RefreshCounter();
            MarkActiveItemAsPurchased();
            CancelPurchase();
        }
        else
        {
            if (audioManager != null) audioManager.PlaySFX(audioManager.noEnergy);
        }
    }

    private void MarkActiveItemAsPurchased()
    {
        ItemUIController[] items = itemsGridParent.GetComponentsInChildren<ItemUIController>();
        items[activeIndex].MarkAsPurchased();
    }

    public void NextShift()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void PurchasePopUp(Item item, int index)
    {
        activeIndex = index;
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
