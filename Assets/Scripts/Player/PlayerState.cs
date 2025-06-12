using System.Collections.Generic;
using System.Diagnostics;

/*
* Player State
* ~~~~~~~~~~~~~
* Information for player status/inventory/abilities
*/
public class PlayerState
{
    /* Office Bucks:
    Only valid within DMV. Purchase Consumables and Supplies
	Rewarded after each shift and review. 
    Rewarded by Events.
	Spent at Shop.
    tems and Unspent are lost when Fired or Reincarnated.
    */
    private int officeBucks = 0;
    /* Soul Credits:
    Real Money. Rent / Certifications / Personal Life
	Rewarded by Reviews and Rare Events.
	Spent in Apartment
	Small amount is lost when Fired or Reincarnated.
    */
    private int soulCredits = 0;

    /* Reputation:
    Reputation tracking for Astaroth, Verinne, and Sothoth. 
    Used by some events and side-quests
    Rewarded by NPC quests and post-combat
    */
    private int aRep = 0;
    private int vRep = 0;
    private int sRep = 0;

    private Class playerClass;
    private List<Action> actionLoadout = new List<Action>();
    private List<Certificate> activeCertificates = new List<Certificate>();
    private List<Furniture> activeFurniture = new List<Furniture>();
    private List<string> itemInventory = new List<string>();
    private List<string> artifacts = new List<string>();

    public List<Action> GetActionLoadout() {
        return actionLoadout;
    }

    public void AddStarterItem()
    {
        artifacts.Add(playerClass.startingItem.ID);
    }

    public void ApplyActionUpgrade(ActionUpgrade upgrade, int actionAppliedTo)
    {
        Action action = actionLoadout[actionAppliedTo];
        action.FRUSTRATION_MODIFIER += upgrade.FRUSTRATION_MODIFIER;
        action.PERFORMANCE_MODIFIER += upgrade.PERFORMANCE_MODIFIER;
        action.WILL_MODIFIER -= upgrade.WILL_MODIFIER;
        foreach (ActionEffectStacks effectStack in upgrade.effects)
        {
            // TODO: fix this to actually check if there is already a stack of the same effect type
            action.effects.Add(effectStack);
        }
        if (upgrade.updateMovement) action.movement = upgrade.movement;
    }

    public void AddActionToLoadout(Action action) {
        actionLoadout.Add(action);
    }

    public void UpdateOfficeBucks(int amount) {
        officeBucks += amount;
    }

    public int GetOfficeBucks() {
        return officeBucks;
    }

    public void UpdateSoulCredits(int amount) {
        soulCredits += amount;
    }

    public int GetSoulCredits() {
        return soulCredits;
    }

    public void UpdateVRep(int amount) {
        vRep += amount;
    }

    public int GetVRep() {
        return vRep;
    }

    public void UpdateARep(int amount) {
        aRep += amount;
    }

    public int GetARep() {
        return aRep;
    }

    public void UpdateSRep(int amount) {
        sRep += amount;
    }

    public int GetSRep() {
        return sRep;
    }

    public void AddCertificate(Certificate cert) {
        activeCertificates.Add(cert);
    }

    public List<Certificate> GetCertificates() {
            return activeCertificates;
    }

    public void AddFurniture(Furniture furn) {
        activeFurniture.Add(furn);
    }

    public List<Furniture> GetFurniture() {
            return activeFurniture;
    }

    public void AddItem(string itemId) {
        itemInventory.Add(itemId);
    }
    public void RemoveItem(string itemId) {
        itemInventory.Remove(itemId);
    }

    public List<string> GetInventory() {
            return itemInventory;
    }

    public List<string> GetArtifacts() {
            return artifacts;
    }

    public void AddArtifact(string itemId) {
        artifacts.Add(itemId);
    }

    public bool ContainsItem(string itemId) {
        return artifacts.Contains(itemId) || itemInventory.Contains(itemId);
    }
    
    public Class GetClass() {
        return playerClass;
    }

    public void UpdateClass(Class playerClass)
    {
        this.playerClass = playerClass;
        actionLoadout = new List<Action>();
    }

    public void ResetRun()
    {
        artifacts = new List<string>();
        itemInventory = new List<string>();
        officeBucks = 0;
    }
}
