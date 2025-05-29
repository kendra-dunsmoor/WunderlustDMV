using System.Collections.Generic;
using UnityEngine;

/*
* Game State
* ~~~~~~~~~~~~~
* Information for game status/progression
*/
public class GameState
{
    private float performance = 50f;
    private float will = 50f;
    private int currentWeekday = 0;
    private List<string> runPath = new List<string>();
    
    public int GetDay() {
        return currentWeekday;
    }

    // Called when combat for day is completed
    public void CompleteDay(float performance, float will) {
        currentWeekday++;
        this.performance = performance;
        this.will = will;
    }

    public void AddRunChoice(string choice) {
        Debug.Log("Adding");
        runPath.Add(choice);
    }
    public List<string> FetchRunPath() {
        return runPath;
    }

    public float GetPerformance() {
        return performance;
    }

    public float GetWill() {
        return will;
    }
}
