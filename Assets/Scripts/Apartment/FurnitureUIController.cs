using UnityEngine;
using TMPro;

public class FurnitureUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject priceTag;
    [SerializeField] private GameObject purchasedTag;

    public Furniture furniture;

    public void AddFurniture(Furniture furn)
    {
        furniture = furn;
        title.text = furn.title;
        if (price != null) purchasedTag.SetActive(false);
        if (price != null) price.text = furn.price.ToString();
        if (gameObject.GetComponent<MouseOverDescription>() != null) {
            gameObject.GetComponent<MouseOverDescription>().UpdateDescription(furn.description);
        }
    }
    public void MarkAsPurchased()
    {
        priceTag.SetActive(false);
        purchasedTag.SetActive(true);
    }

    public void PurchaseScreen(FurnitureUIController furn)
    {
        FindFirstObjectByType<FurniturePageController>().PurchasePopUp(furn.furniture);
    }
}
