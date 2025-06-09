using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
* Calendar Controller
* ~~~~~~~~~~~~~~~~~~~~
* Transition screen between shifts/events/shop. Populate with current run
* state and present two choices to player after morning shift.
*/
public class CalendarController : MonoBehaviour
{
    [SerializeField] CalendarDay[] calendarDays;
    [SerializeField] TextMeshProUGUI optionA;
    [SerializeField] TextMeshProUGUI optionB;
    [SerializeField] Sprite emptySpotImage;
    [SerializeField] Sprite stickyNoteImage;
    [SerializeField] Color emptySpotColor;
    [SerializeField] GameObject choiceOffering;
    [SerializeField] Button performanceReviewButton;


    private GameManager gameManager;
    private SceneFader sceneFader;

    private int currDay = 0;

    private AudioManager audioManager;

    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();        
    }

    void Start()
    {
        sceneFader = FindFirstObjectByType<SceneFader>();
        sceneFader.transform.SetAsLastSibling(); // See if this layers it on top
        if (audioManager != null) audioManager.PlaySFX(audioManager.paperRustle);
        if (gameManager != null)
        {
            currDay = gameManager.FetchCurrentCalendarDay();
        }
        else
        {
            Debug.LogError("No game manager found");
            currDay = 1;
        }
        FillCalendar();
    }
    
    private void FillCalendar()
    {
        Debug.Log("Fill Calendar for day: " + currDay);
        // TODO: if end of week enable vending machine button
        // also remove hover change when not day 5
        choiceOffering.SetActive(currDay != 5);
        performanceReviewButton.interactable = currDay == 5;

        // Set options for current shift
        calendarDays[currDay - 1].shiftCompleteMarker.SetActive(true);
        calendarDays[currDay - 1].choiceMarker.SetActive(true);
        SetToEmptySlot(calendarDays[currDay - 1].choiceMarker);
        optionA.text = gameManager.FetchNextShiftChoice();
        optionB.text = gameManager.FetchNextShiftChoice();

        // Fetch past run choices and fill calendar
        List<string> runPath = gameManager.FetchRunPath();
        if (runPath == null) return;
        for (int i = 0; i < runPath.Count; i++)
        {
            calendarDays[i].shiftCompleteMarker.SetActive(true);
            calendarDays[i].choiceMarker.SetActive(true);
            SetSlotToChoiceMade(calendarDays[i].choiceMarker, runPath[i]);
        }
    }

    private void SetToEmptySlot(GameObject noteSlot)
    {
        noteSlot.GetComponent<Image>().sprite = emptySpotImage;
        noteSlot.GetComponentInChildren<TextMeshProUGUI>().text = "Select Break Activity";
        noteSlot.GetComponentInChildren<TextMeshProUGUI>().color = emptySpotColor;
    }

    private void SetSlotToChoiceMade(GameObject noteSlot, string choice)
    {
        noteSlot.GetComponent<Image>().sprite = stickyNoteImage;
        noteSlot.GetComponentInChildren<TextMeshProUGUI>().text = choice;        
    }

    public void SelectChoice(TextMeshProUGUI choice)
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.paperRustle);
        Debug.Log("Selecting choice: " + choice.text);
        gameManager.StoreRunChoice(choice.text);
        switch (choice.text)
        {
            case "<u>Break Room</u>":
                BreakRoom();
                break;
            case "<u>Office Event</u>":
                TriggerEvent();
                break;
            case "<u>Vending Machine</u>":
                GoToVendingMachine();
                break;
        }
    }
    private void GoToVendingMachine() {
        sceneFader.LoadScene(6);
    }

    private void BreakRoom() {
        sceneFader.LoadScene(2);
    }

    private void TriggerEvent() {
        sceneFader.LoadScene(4);
    }
}
