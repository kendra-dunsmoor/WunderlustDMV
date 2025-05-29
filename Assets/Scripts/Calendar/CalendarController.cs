using System.Collections.Generic;
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
    private int currDay = 0;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        // gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gameManager != null) {
            currDay = gameManager.FetchCurrentCalendarDay();
        } else {
            Debug.LogError("No game manager found");
            currDay = 1;
        }
        // TODO: if end of week skip calendar for now go straight to performance review
        if (currDay == 5) SceneManager.LoadSceneAsync(5);
        else FillCalendar();
    }
    private void FillCalendar() {
        Debug.Log("Fill Calendar for day: " + currDay);
        // Set options for current shift
        calendarDays[currDay - 1].shiftCompleteMarker.SetActive(true);
        calendarDays[currDay - 1].orText.SetActive(true);
        calendarDays[currDay - 1].optionA.SetActive(true);
        calendarDays[currDay - 1].optionB.SetActive(true);
        calendarDays[currDay - 1].optionA.GetComponentInChildren<TextMeshProUGUI>().text = gameManager.FetchNextShiftChoice();
        calendarDays[currDay - 1].optionB.GetComponentInChildren<TextMeshProUGUI>().text = gameManager.FetchNextShiftChoice();
        
        // Fetch past run choices and fill calendar
        List<string> runPath = gameManager.FetchRunPath();
        if (runPath == null) return;
        for (int i = 0; i < runPath.Count; i++) {
            calendarDays[i].shiftCompleteMarker.SetActive(true);
            // TODO: only storing string rn so just adding to first note, not which (top or bottom) player actually picked
            calendarDays[i].orText.SetActive(false);
            calendarDays[i].optionB.SetActive(false);
            calendarDays[i].optionA.SetActive(true);
            calendarDays[i].optionA.GetComponentInChildren<TextMeshProUGUI>().text = runPath[i];
        }
    }

    public void SelectChoice(TextMeshProUGUI choice) {
        Debug.Log("Selecting choice: " + choice.text);
        gameManager.StoreRunChoice(choice.text);
        switch (choice.text) {
            case "Break Room":
                BreakRoom();
                break;
            case "Event":
                TriggerEvent();
                break;
            case "Vending Machine":
                GoToVendingMachine();
                break;
        }
    }
    private void GoToVendingMachine() {
        SceneManager.LoadSceneAsync(6);
    }

    private void BreakRoom() {
        SceneManager.LoadSceneAsync(2);
    }

    private void TriggerEvent() {
        SceneManager.LoadSceneAsync(4);
    }
}
