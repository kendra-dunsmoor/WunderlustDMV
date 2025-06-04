using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;

/*
* Game State
* ~~~~~~~~~~~~~
* Information for game status/progression
*/
public class GameState
{
    private float performance = 100f;
    private float will = 100f;
    private int currentWeekday = 0;
    private List<string> runPath = new List<string>();

    public enum RunStatus {
        ACTIVE,
        FIRED,
        REINCARNATED,
        WON
    }

    private RunStatus runStatus = RunStatus.ACTIVE;
    
    public int GetDay() {
        return currentWeekday;
    }

    // Called when combat for day is completed
    public void CompleteDay(float performance, float will) {
        currentWeekday++;
        this.performance = performance;
        this.will = will;
    }

    public RunStatus GetRunStatus() {
        return runStatus;
    }

    // Called when combat for day is completed
    public void UpdateRunStatus(RunStatus result) {
        runStatus = result;
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

    public void ResetRun() {
        will = 100f;
        performance = 100f;
        runPath = new List<string>();
        currentWeekday = 0;
    }
}
