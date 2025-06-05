using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Runtime.InteropServices;

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
    public bool inTutorial;

    // temporary for events:
    private List<string> eventChoices = new List<string> {"Vending Machine", "Break Room", "Office Event"};

    // temporary until can configure loadout in apartment
    [SerializeField, Tooltip("If loadout not customized from apartment use base")] private List<Action> STARTER_LOADOUT;

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

        // temp:
        foreach ( Action action in STARTER_LOADOUT) {
            playerStatus.AddActionToLoadout(Instantiate(action.GetCopy()));
        }
    }

    public int FetchCurrentCalendarDay() {
        // temp just return current day for calendar
        return gameStatus.GetDay();
    }

    public string FetchNextShiftChoice() {
        // TODO: improve this for select shift choice for run
        int choice = Random.Range(0, 3);
        Debug.Log("Choice: " + choice);
        // temp just testing vending machine:
        return eventChoices[choice];
    }

    public void ShiftCompleted(float performance, float will) {
        // TODO: update rewards for player inventory and game state
        gameStatus.CompleteDay(performance, will);
    }

    public void StoreRunChoice(string choice) {
        gameStatus.AddRunChoice(choice);
    }
    public List<string> FetchRunPath() {
        return gameStatus.FetchRunPath();
    }

    public void RestartRun() {
        // Back to apartment, reset certain run only trackers
        playerStatus.ResetRun();
        gameStatus.ResetRun();
    }

    public GameState.RunStatus FetchRunState() {
        return gameStatus.GetRunStatus();
    }

    public void UpdateRunStatus(GameState.RunStatus state) {
        gameStatus.UpdateRunStatus(state);
    }

    public int FetchOfficeBucks() {
        return playerStatus.GetOfficeBucks();
    }
    public void UpdateOfficeBucks(int amount) {
        Debug.Log("Adding office bucks + " + amount);
        playerStatus.UpdateOfficeBucks(amount);
        GameObject counter = GameObject.FindGameObjectWithTag("Counter_OfficeBucks");
        if (counter != null) counter.GetComponentInChildren<TextMeshProUGUI>().text = playerStatus.GetOfficeBucks().ToString();
    }

    public float FetchWill() {
        return gameStatus.GetWill();
    }

    public float FetchPerformance() {
        return gameStatus.GetPerformance();
    }

    public int FetchSoulCredits() {
        return playerStatus.GetSoulCredits();
    }

    public void UpdateSoulCredits(int change) {
        playerStatus.UpdateSoulCredits(change);
        GameObject counter = GameObject.FindGameObjectWithTag("Counter_SoulCredits");
        if (counter != null) counter.GetComponentInChildren<TextMeshProUGUI>().text = playerStatus.GetOfficeBucks().ToString();
    }

    public List<Certificate> FetchCertificates() {
        return playerStatus.GetCertificates();
    }

    public void AddCertificate(Certificate cert) {
        playerStatus.AddCertificate(cert);
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
        return itemDatabase.GetItemCopy(id);
    }

    public List<Item> FetchRandomItems(int numItems, bool shouldBeArtifact) {
        return itemDatabase.GetRandomItems(numItems, shouldBeArtifact);
    }

    // TODO: improve random selection with rarity values:

    // private int GetRandomItem() {
    //     float totalChance = 0f;
    //     int maxCharacter = Characters.Count < Manager.GetLevel() + 1 ? Characters.Count : Manager.GetLevel() + 1;
    //     for (int i = 0; i < maxCharacter; i++)
    //     {
    //         totalChance += Characters[i].GetComponent<Character>().getSpawnRate();
    //     }
    //     float rand = Random.Range(0f, totalChance);
    //     float cumulativeChance = 0f;
    //     for (int i = 0; i < maxCharacter; i++)
    //     {
    //         cumulativeChance += Characters[i].GetComponent<Character>().getSpawnRate();
    //         if (rand <= cumulativeChance)
    //         {
    //             Debug.Log("Spawn: " + i);
    //             return i;
    //         }
    //     }
    //     return 0;
    // }

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
