using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassUIController : MonoBehaviour
{
    [SerializeField] Image startingItemImage;
    [SerializeField] Image specialActionImage;
    [SerializeField] TextMeshProUGUI className;
    
    public void AddClass(Class playerClass)
    {
        className.text = playerClass.className;

        startingItemImage.sprite = playerClass.startingItem.Icon;
        startingItemImage.GetComponent<MouseOverDescription>()
            .UpdateDescription(playerClass.startingItem.description, playerClass.startingItem.itemName);

        specialActionImage.sprite = playerClass.specialAction.baseButtonImage;
        specialActionImage.GetComponent<MouseOverDescription>()
            .UpdateDescription(playerClass.specialAction.GetDescription(), playerClass.specialAction.actionName);
    }
}
