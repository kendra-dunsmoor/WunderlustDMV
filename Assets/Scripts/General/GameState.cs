using System.Collections.Generic;
using UnityEngine;

/*
* Game State
* ~~~~~~~~~~~~~
* Information for game status/progression
*/
public class GameState
{
    private int currentWeek = 0;
    private int currentWeekday = 0;
    private List<string> runPath = new List<string>();


    private enum Location {
        APARTMENT,
        DESK,
        BREAK_ROOM,
        EVENT
    }

    private Location currentLocation = Location.APARTMENT;
    
    public int GetDay() {
        return currentWeekday;
    }

    // Called when combat for day is completed
    public void CompleteDay() {
        currentWeekday++;
    }

    public void AddRunChoice(string choice) {
        Debug.Log("Adding");
        runPath.Add(choice);
    }
    public List<string> FetchRunPath() {
        return runPath;
    }
}
