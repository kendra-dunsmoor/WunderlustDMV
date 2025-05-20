using UnityEngine;
using System.Collections.Generic;


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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int FetchCurrentCalendarDay() {
        // temp just return current day for calendar
        return gameStatus.GetDay();
    }

    public string FetchNextShiftChoice() {
        // TODO: randomly select shift choice for run
        return "Event";
    }

    public void ShiftCompleted(float performance, float will) {
        // TODO: update rewards for player inventory and game state
        gameStatus.CompleteDay(performance, will);
    }

    public void StoreRunChoice(string choice) {
        Debug.Log("Storing");
        gameStatus.AddRunChoice(choice);
    }
    public List<string> FetchRunPath() {
        return gameStatus.FetchRunPath();
    }

    public void RestartRun() {
        //TODO
    }

    public void UpdateOfficeBucks(int amount) {
        Debug.Log("Adding office bucks + " + amount);
        playerStatus.UpdateOfficeBucks(amount);
    }

    public float FetchWill() {
        return gameStatus.GetWill();
    }

    public float FetchPerformance() {
        return gameStatus.GetPerformance();
    }
}
