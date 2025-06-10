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
    private float maxWill = 100f;
    private float performance = 100f;
    private float will = 100f;
    private float attention = 20f;
    private int currentWeekday = 0;
    private int earlyShifts = 0;
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
    public void CompleteDay() {
        currentWeekday++;
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

    public float GetAttention() {
        return attention;
    }
    
        
    public float GetEarlyShifts() {
        return earlyShifts;
    }

    public float GetWill() {
        return will;
    }
    
    public float GetMaxWill()
    {
        return maxWill;
    }

    public void UpdatePerformance(float performance)
    {
        this.performance = performance;
    }

     public void UpdateAttention(float attention)
    {
        this.attention = attention;
    }

    public void UpdateWill(float will)
    {
        this.will = will;
    }

    public void EarlyShift()
    {
        earlyShifts++;
    }
    
     public void UpdateMaxWill(float maxWill)
    {
        this.maxWill = maxWill;
    }

    public void ResetRun()
    {
        will = 100f;
        performance = 100f;
        attention = 20f;
        runPath = new List<string>();
        currentWeekday = 0;
        earlyShifts = 0;
    }
}
