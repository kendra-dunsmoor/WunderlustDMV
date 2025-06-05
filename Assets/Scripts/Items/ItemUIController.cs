using UnityEngine;
using TMPro;

public class ItemUIController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private GameObject priceTag;
    [SerializeField] private GameObject purchasedTag;
    [SerializeField] private GameObject itemImage;
    public int itemIndex;

    public Item item;

    public void AddItem(Item item, int index)
    {
        Debug.Log("Adding item for vending UI");
        this.item = item;
        itemIndex = index;
        purchasedTag.SetActive(false);
        price.text = item.price.ToString();
        if (gameObject.GetComponent<MouseOverDescription>() != null) {
            gameObject.GetComponent<MouseOverDescription>().UpdateDescription(item.description);
        }
        itemImage.GetComponent<UnityEngine.UI.Image>().sprite = item.Icon;
    }

    public void MarkAsPurchased()
    {
        Debug.Log("Marking as purchased");
        if (priceTag != null) priceTag.SetActive(false);
        if (priceTag != null) purchasedTag.SetActive(true);
    }

    public void PurchaseScreen(ItemUIController itemUI)
    {
        if (!purchasedTag.activeInHierarchy)
            FindFirstObjectByType<VendingMachineController>().PurchasePopUp(itemUI.item, itemIndex);
    }

    public void AddItemUIForPurchase(Item item)
    {
        Debug.Log("Adding for purchase UI");
        this.item = item;
        itemImage.GetComponent<UnityEngine.UI.Image>().sprite = item.Icon;
    }
}
