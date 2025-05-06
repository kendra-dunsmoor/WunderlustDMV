using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalendarController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI optionA;
    [SerializeField] TextMeshProUGUI optionB;
    private GameManager gameManager;

    void Start()
    {
        Debug.Log("Called at Instantiate");
        gameManager = FindFirstObjectByType<GameManager>();
        GetShiftOptions();
    }

    private void GetShiftOptions() {
        // Fetch past run choices and fill calendar
        // Fetch options for current shift
        optionA.text = gameManager.FetchNextShiftChoice();
        optionB.text = gameManager.FetchNextShiftChoice();
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
