using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] ItemInventory inventory;
    private GameManager gameManager;
    private CombatManager combatManager;

    // TODO: maybe add artifacts panel here too? Idk if it should be separate

    private void Awake() {
        inventory.OnItemClickedEvent += InventoryClick;
    }

    private void Start()
	{
        // Get Game Manager for updating inventory
        gameManager = FindFirstObjectByType<GameManager>();
        combatManager = FindFirstObjectByType<CombatManager>();
		// Get curr inventory of player to update UI
        LoadInventory();
	}

    private void LoadInventory() {
        Debug.Log("Fetching inventory");
        // fetch player inventory list of ids
        List<string> itemIds = gameManager.FetchInventory();
        Debug.Log("Found " + itemIds.Count + " ids");
        // grab those ids from db
        foreach (string id in itemIds) {
            inventory.AddItem(gameManager.GetItemFromDB(id));
        }
    }

    public void InventoryClick(Item item) {
        // TODO add item effect
        if (item is UsableItem) {
            UsableItem usableItem = (UsableItem) item;
			// usableItem.Use();
            // TODO: remove hard coded effects here
            switch (item.itemName){
                case "Break Room Coffee":
                    combatManager.UpdateWill(-5);
                    combatManager.AddNewEffects(item.effects[0], 2);
                    break;
                case "GreenCow":
                    combatManager.UpdateWill(-10);
                    combatManager.AddNewEffects(item.effects[0], 3);
                    break;
                case "Carca-Cola":
                    combatManager.UpdateWill(-10);
                    break;
            }
        }
        // TODO: add ability to stack
        inventory.RemoveItem(item);
        gameManager.RemoveFromInventory(item.ID);
    }
}
