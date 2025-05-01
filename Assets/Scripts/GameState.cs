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
    private string[] weekdays = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday"};

    private enum Location {
        APARTMENT,
        DESK,
        BREAK_ROOM,
        EVENT
    }

    private Location currentLocation = Location.APARTMENT;
    
    public string GetDay() {
        return weekdays[currentWeekday];
    }

// Called when combat for day is completed
    public string CompleteDay() {
        return weekdays[currentWeekday];
    }
}
