using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
* Calendar Controller
* ~~~~~~~~~~~~~~~~~~~~
* Transition screen between shifts/events/shop. Populate with current run
* state and present two choices to player after morning shift.
*/
public class CalendarController : MonoBehaviour
{
    [SerializeField] CalendarDay[] calendarDays;
    private GameManager gameManager;
    private int currDay;

    void Start()
    {
        Debug.Log("Called at Instantiate");
        gameManager = FindFirstObjectByType<GameManager>();
        currDay = gameManager.FetchCurrentCalendarDay();
        FillCalendar();
    }
    private void FillCalendar() {
        // Fetch past run choices and fill calendar
        for (int i = 0; i < currDay; i++) {
            calendarDays[i].shiftCompleteMarker.enabled = true;
        }
        // Fetch options for current shift
        calendarDays[currDay].optionA.text = gameManager.FetchNextShiftChoice();
        calendarDays[currDay].optionB.text = gameManager.FetchNextShiftChoice();
    }

    public void SelectChoice(TextMeshProUGUI choice) {
        // TODO: store choice and update scene
        if (choice.text == "Break Room") BreakRoom();
        if (choice.text == "Event") TriggerEvent();
    }

    private void BreakRoom() {
        SceneManager.LoadSceneAsync(2);
    }

    private void TriggerEvent() {
        SceneManager.LoadSceneAsync(4);
    }
}
