using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/*
* Game Manager
* ~~~~~~~~~~~~~
* Repsonsible for saving cross scene player status/progression logic
*/
public class GameManager : MonoBehaviour
{
    // ~~~~~~ Game State ~~~~~~~
    public static GameManager instance;
    private GameState gameStatus;
    private PlayerState playerStatus;
    [SerializeField] private ItemDB itemDatabase;

    // temp solution until events are expanded:
    private bool inTutorial;

    // temporary for events:
    private List<string> eventChoices = new List<string> {"Vending Machine", "Break Room", "Office Event"};

    // temporary until can configure loadout in apartment
    [SerializeField, Tooltip("If loadout not customized from apartment use base")] private Class STARTER_CLASS;

    // ~~~~~~ Functions ~~~~~~
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // TODO: logic to fetch for saved game vs new game
        gameStatus = new GameState();
        playerStatus = new PlayerState();

        // temp:
        inTutorial = true;
    }

    public void StartRun()
    {
        List<Certificate> playerCerts = FetchCertificates();
        List<Furniture> playerFurn = FetchFurniture();

        // Landlord takes rest of soul credits
        int rent = FetchSoulCredits();
         if (playerCerts.Any(c => c.type == Certificate.CertificateType.FINANCIAL_LITERACY))
		{
            if (rent>10) rent -= 10;
            else rent = 0;
		}
        UpdateSoulCredits(-rent);

        // Clear last run outcome status
        gameStatus.UpdateRunStatus(GameState.RunStatus.ACTIVE);
        
        // Setup class from computer selection or default if no choice made
        if (playerStatus.GetClass() == null)
        {
            playerStatus.UpdateClass(STARTER_CLASS);
            foreach (Action action in playerStatus.GetClass().actionLoadout)
            {
                playerStatus.AddActionToLoadout(Instantiate(action.GetCopy()));
            }
        }
        playerStatus.AddStarterItem(); // Initialize empty inventory + starter artifact
    }

    public int FetchCurrentCalendarDay()
    {
        // temp just return current day for calendar
        return gameStatus.GetDay();
    }
    
    public bool InTutorial() {
        return inTutorial;
    }

    public void UpdateTutorialStatus(bool status) {
        inTutorial = status;
    }

    public string FetchNextShiftChoice()
    {
        // Tutorial force break room for Sothoth event
        if (inTutorial) return eventChoices[1];
        // TODO: improve this for select shift choice for run
        int choice = Random.Range(0, 3);
        return eventChoices[choice];
    }

    public void ShiftCompleted(float performance, float will, float attention)
    {
        gameStatus.CompleteDay();
        gameStatus.UpdatePerformance(performance);
        gameStatus.UpdateWill(will);
        gameStatus.UpdateAttention(attention);
    }
    public void EarlyShift()
    {
        gameStatus.EarlyShift();
    }

    public void RunWon()
    {
        gameStatus.UpdateRunStatus(GameState.RunStatus.WON);
        playerStatus.ResetRun();
        foreach ( Action action in playerStatus.GetClass().actionLoadout) {
            playerStatus.AddActionToLoadout(Instantiate(action.GetCopy()));
        }
        gameStatus.ResetRun();
    }

    public void StoreRunChoice(string choice)
    {
        gameStatus.AddRunChoice(choice);
    }
    public List<string> FetchRunPath() {
        return gameStatus.FetchRunPath();
    }

    public void RestartRun() {
        // Back to apartment, reset certain run only trackers
        playerStatus.ResetRun();
        foreach ( Action action in playerStatus.GetClass().actionLoadout) {
            playerStatus.AddActionToLoadout(Instantiate(action.GetCopy()));
        }
        gameStatus.ResetRun();
    }

    public GameState.RunStatus FetchRunState() {
        return gameStatus.GetRunStatus();
    }

    public void UpdateRunStatus(GameState.RunStatus state) {
        gameStatus.UpdateRunStatus(state);
    }
    public Class FetchPlayerClass() {
        return playerStatus.GetClass();
    }

    public void UpdatePlayerClass(Class playerClass)
    {
        Debug.Log("Updating class to + " + playerClass.className);
        playerStatus.UpdateClass(playerClass);
        foreach ( Action action in playerStatus.GetClass().actionLoadout) {
            playerStatus.AddActionToLoadout(Instantiate(action.GetCopy()));
        }
    }

    public int FetchOfficeBucks()
    {
        return playerStatus.GetOfficeBucks();
    }
    
    public void UpdateOfficeBucks(int amount)
    {
        Debug.Log("Adding office bucks + " + amount);
        playerStatus.UpdateOfficeBucks(amount);
        GameObject counter = GameObject.FindGameObjectWithTag("Counter_OfficeBucks");
        if (counter != null) counter.GetComponentInChildren<TextMeshProUGUI>().text = playerStatus.GetOfficeBucks().ToString();
    }

    public float FetchWill() {
        return gameStatus.GetWill();
    }
    public void UpdateWill(float will)
    {
        gameStatus.UpdateWill(will);
    }

    public float FetchMaxWill() {
           return gameStatus.GetMaxWill();
    }

    public void UpdateMaxWill(float maxWill)
    {
        gameStatus.UpdateWill(maxWill);
    }

    public float FetchAttention()
    {
        return gameStatus.GetAttention();
    }

    public void UpdateAttention(float attention)
    {
        gameStatus.UpdateAttention(attention);
    }

    public float FetchPerformance()
    {
        return gameStatus.GetPerformance();
    }
    public void UpdatePerformance(float performance)
    {
        gameStatus.UpdatePerformance(performance);
    }

    public int FetchSoulCredits()
    {
        return playerStatus.GetSoulCredits();
    }

    public void UpdateSoulCredits(int change) {
        playerStatus.UpdateSoulCredits(change);
        GameObject counter = GameObject.FindGameObjectWithTag("Counter_SoulCredits");
        if (counter != null) counter.GetComponentInChildren<TextMeshProUGUI>().text = playerStatus.GetSoulCredits().ToString();
    }

    public int FetchVRep()
    {
        return playerStatus.GetVRep();
    }

    public void UpdateVRep(int change) {
        playerStatus.UpdateVRep(change);
        // GameObject counter = GameObject.FindGameObjectWithTag("Counter_VRep");
        // if (counter != null) counter.GetComponentInChildren<TextMeshProUGUI>().text = playerStatus.GetVRep().ToString();
    }

     public int FetchARep()
    {
        return playerStatus.GetARep();
    }

    public void UpdateARep(int change) {
        playerStatus.UpdateARep(change);
        // GameObject counter = GameObject.FindGameObjectWithTag("Counter_ARep");
        // if (counter != null) counter.GetComponentInChildren<TextMeshProUGUI>().text = playerStatus.GetARep().ToString();
    }


    public List<Certificate> FetchCertificates() {
        return playerStatus.GetCertificates();
    }

    public void AddCertificate(Certificate cert) {
        playerStatus.AddCertificate(cert);
    }

    public List<Furniture> FetchFurniture() {
        return playerStatus.GetFurniture();
    }

    public void AddFurniture(Furniture furn) {
        playerStatus.AddFurniture(furn);
    }

    public List<string> FetchInventory() {
        return playerStatus.GetInventory();
    }

    public void AddToInventory(string id) {
        Debug.Log("Adding item with id: " + id + " to player inventory");
        playerStatus.AddItem(id);
    }

    public void RemoveFromInventory(string id) {
        Debug.Log("Removing item with id: " + id + " from player inventory");
        playerStatus.RemoveItem(id);
    }

    public List<string> FetchArtifacts() {
        return playerStatus.GetArtifacts();
    }

    public void AddArtifact(string id) {
        playerStatus.AddArtifact(id);
    }

    public Item GetItemFromDB(string id) {
        return Instantiate(itemDatabase.GetItemCopy(id));
    }

    public List<Item> FetchRandomItems(int numItems, bool shouldBeArtifact) {
        return itemDatabase.GetRandomItems(numItems, shouldBeArtifact);
    }

    public bool ContainsItem(string itemId) {
        return playerStatus.ContainsItem(itemId);
    }

    public List<Action> FetchActions() {
        return playerStatus.GetActionLoadout();
    }

    public void ApplyActionUpgrade(ActionUpgrade upgrade, int actionAppliedTo) {
        playerStatus.ApplyActionUpgrade(upgrade, actionAppliedTo);
    }
}
