using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] ItemInventory inventory;
    [SerializeField] ItemInventory artifacts;
    private GameManager gameManager;
    private CombatManager combatManager;
    private AudioManager audioManager;

    // TODO: maybe add artifacts panel here too? Idk if it should be separate

    private void Awake() {
        inventory.OnItemClickedEvent += InventoryClick;
    }

    private void Start()
	{
        // Get Game Manager for updating inventory
        gameManager = FindFirstObjectByType<GameManager>();
        audioManager = FindFirstObjectByType<AudioManager>();
        combatManager = FindFirstObjectByType<CombatManager>();
		// Get curr inventory of player to update UI
        if (gameManager != null) {
            LoadInventory();
            LoadArtifacts();
        }
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
        if (combatManager.actionsDisabled) return;
        if (item is UsableItem) {
            if (audioManager != null) audioManager.PlaySFX(audioManager.drink);
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
        Debug.Log("Incrementing artifacts");
        for (int i = 0; i < artifacts.items.Count; i++)
        {
            ArtifactItem item = (ArtifactItem) artifacts.items[i];
            item.currentTurnCounter++;
            // increment turns in hover description
            if (!item.isEndOfShiftEffect)
            {
                artifacts.UpdateDescription(
                    i,
                    item.description + "\n Turn Counter: " + item.currentTurnCounter + "/" +item.turnClock,
                    item.itemName
                );   
            }
            if (item.currentTurnCounter == item.turnClock)
            {
                item.currentTurnCounter = 0;
                UseArtifact(item);
            }
        }
    }

    private void UseArtifact(ArtifactItem item) {
        Debug.Log("Use artifact: " + item.itemName);
        // TODO: should there be a sound here? Maybe not
        if (audioManager != null) audioManager.PlaySFX(audioManager.specialActionButton);
        // Add item effects
        AddEffectsAndModifiers(item);
        // Note: a couple artifacts are not turn based and are hard coded in combat manager end of shift effects for now
        
    }

    // Check for specific end of shift incremented artifacts:
    public void EndShiftArtifacts() {
        foreach (ArtifactItem item in artifacts.items) {
            if (item.isEndOfShiftEffect) UseArtifact(item);
        }
    }

    private void AddEffectsAndModifiers(Item item) {
        foreach (ActionEffectStacks effectStack in item.effects) {
            if (effectStack.stacks < 0) combatManager.RemoveEffectStacks(effectStack.stacks, effectStack.effect.type);
            else combatManager.AddNewEffect(effectStack.effect, effectStack.stacks);
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
