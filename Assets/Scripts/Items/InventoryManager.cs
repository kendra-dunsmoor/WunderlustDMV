using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] ItemInventory inventory;
    [SerializeField] ItemInventory artifacts;
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
        LoadArtifacts();
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

    private void LoadArtifacts() {
        Debug.Log("Fetching artifacts");
        // fetch player artifacts list of ids
        List<string> artifactIds = gameManager.FetchArtifacts();
        Debug.Log("Found " + artifactIds.Count + " ids");
        // grab those ids from db
        foreach (string id in artifactIds) {
            artifacts.AddItem(gameManager.GetItemFromDB(id));
        }
    }

    public void InventoryClick(Item item) {
        if (item is UsableItem) {
            UsableItem usableItem = (UsableItem) item;

            AddEffectsAndModifiers(item);
            // Add item effects

            // Any special cases outside of general effects/modifiers will need to be added by name here
            switch (item.itemName){
                case "Herbal Tea":
                    combatManager.ClearPlayerConditions();
                    break;
            }
            // TODO: add ability to stack
            inventory.RemoveItem(item);
            gameManager.RemoveFromInventory(item.ID);
        }
    }

    public void IncrementArtifacts() {
        foreach (ArtifactItem item in artifacts.items) {
            item.currentTurnCounter++;
            if (item.currentTurnCounter == item.turnClock)
                UseArtifact(item);
        }
    }

    private void UseArtifact(ArtifactItem item) {
        // Add item effects
        AddEffectsAndModifiers(item);

        // Any special cases outside of general effects/modifiers will need to be added by name here
        switch (item.itemName){
            case "TPS Report":
                combatManager.RemoveAttention(1);
                break;
            case "Flair Buttons":
                combatManager.RemoveAttention(1);
                break;
        // Note: a couple artifacts are not turn based and are hard coded in combat manager end of shift effects for now
        }
    }

    // Check for specific end of shift incremented artifacts:
    public void EndShiftArtifacts() {
        foreach (ArtifactItem item in artifacts.items) {
            if (item.itemName == "Fuzzy Dice")
                combatManager.UpdateWill(item.willModifier);
        }
    }

    private void AddEffectsAndModifiers(Item item) {
        foreach (ActionEffect effect in item.effects) {
            combatManager.AddNewEffects(effect, 1); // TODO: fix to store turns for each effect
        }

        // Add item modifiers:
        if (item.willModifier != 0)
            combatManager.UpdateWill(item.willModifier);
        if (item.performanceModifier != 0)
            combatManager.UpdatePerformance(item.performanceModifier);
        if (item.frustrationModifier != 0)
            combatManager.UpdateFrustration(item.frustrationModifier);
    }
}
