using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;

public class ItemUIController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private GameObject priceTag;
    [SerializeField] private GameObject purchasedTag;
    [SerializeField] private GameObject itemImage;

    public Item item;

    public void AddItem(Item item)
    {
        Debug.Log("Adding item for vending UI");
        this.item = item;
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
        priceTag.SetActive(false);
        purchasedTag.SetActive(true);
    }

    public void PurchaseScreen(ItemUIController itemUI)
    {
        FindFirstObjectByType<VendingMachineController>().PurchasePopUp(itemUI.item);
    }

    public void AddItemUIForPurchase(Item item)
    {
        Debug.Log("Adding for purchase UI");
        this.item = item;
        itemImage.GetComponent<UnityEngine.UI.Image>().sprite = item.Icon;
    }
}
